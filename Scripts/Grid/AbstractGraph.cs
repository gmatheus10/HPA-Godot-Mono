
using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
public class AbstractGraph : Node2D
{
  public Grid<Cell> grid;
  private float CellSize;

  public int Level;

  public Vector2 LevelOneClusterSize;

  private Cluster[,] clustersLevel1;
  private Cluster[,] multiLevelCluster;
  public List<Cluster[,]> allClustersAllLevels = new List<Cluster[,]>();

  public Dictionary<string, Entrance> setOfEntrances = new Dictionary<string, Entrance>();

  public void Start(Grid<Cell> Grid, Vector2 LevelOneClusterSize, int Level)
  {
    this.grid = Grid;
    this.CellSize = grid.CellSize;
    this.Level = Level;
    this.LevelOneClusterSize = LevelOneClusterSize;

  }
  public void PreProcessing(int maxLevel)
  {
    AbstractMaze();
    BuildGraph();
    for (int l = 2; l <= maxLevel; l++)
    {
      AddLevelToGraph(l);
    }
  }
  void AbstractMaze()
  {
    clustersLevel1 = BuildClusters();
    for (int i = 0; i < clustersLevel1.GetLength(0); i++)
    {
      for (int j = 0; j < clustersLevel1.GetLength(1); j++)
      {
        Cluster c1 = clustersLevel1[i, j];

        for (int l = -1; l <= 1; l++)
        {
          for (int m = -1; m <= 1; m++)
          {
            //checks only up, down, left and right
            if ((l == 0 && m == 0) || (l == 1 && m == 1) || (l == -1 && m == -1) || (l == -1 && m == 1) || (l == 1 && m == -1))
            {
              continue;
            }
            if ((i + l < 0) || (i + l) >= clustersLevel1.GetLength(0))
            {
              continue;
            }
            if ((j + m < 0) || (j + m) >= clustersLevel1.GetLength(1))
            {
              continue;
            }
            Cluster nextCluster = clustersLevel1[i + l, j + m];
            HandlePairOfClusters(c1, nextCluster);
          }
        }

      }
    }

    void HandlePairOfClusters(Cluster c1, Cluster nextCluster)
    {
      KeyValuePair<string, List<Cell>>[] border = GetAdjacentBorder(c1, nextCluster);

      BuildEntrances(c1, nextCluster, border);

    }

  }
  public Cluster[,] BuildClusters(int level = 1)
  {
    int clustersOnX = 0, clustersOnY = 0;
    GetClusterCount(level, out clustersOnX, out clustersOnY);

    Cluster[,] setOfClusters = new Cluster[clustersOnX, clustersOnY];
    Cluster[,] lesserClustersArray = clustersLevel1;
    for (int x = 0; x < clustersOnX; x++)
    {
      for (int y = 0; y < clustersOnY; y++)
      {
        Cluster cluster = InstantiateCluster(level, x, y, lesserClustersArray);
        setOfClusters[x, y] = cluster;
        FillCluster(cluster);
      }
    }
    clustersLevel1 = setOfClusters;
    return setOfClusters;

    void GetClusterCount(int Level, out int clustersX, out int clustersY)
    {
      clustersX = (int)(grid.Width / (CellSize * LevelOneClusterSize.x));
      clustersY = (int)(grid.Height / (CellSize * LevelOneClusterSize.y));
      if (Level > 1)
      {
        for (int i = 1; i <= Level; i++)
        {
          clustersOnX /= i;
          clustersOnY /= i;
        }
      }
    }
    Cluster InstantiateCluster(int L, int x, int y, Cluster[,] lesser)
    {
      // int levelAdjustmentX = (int)(LevelOneClusterSize.x * Mathf.Pow(2, L - 1));
      // int levelAdjustmentY = (int)(LevelOneClusterSize.y * Mathf.Pow(2, L - 1));

      // Vector2 size = new Vector2(levelAdjustmentX, levelAdjustmentY);
      Vector2 size = Utils.factorial(L) * LevelOneClusterSize;
      Vector2 worldSize = size * CellSize;
      Vector2 clusterPosition = grid.GetWorldPosition((int)(x * size.x), (int)(y * size.y));

      Cluster cluster = new Cluster(size, clusterPosition, L);

      cluster.SetGridPosition(grid.GetGridPosition(clusterPosition));
      cluster.SetWorldSize(size * CellSize);

      if (level > 1)
      {
        cluster.AddLesserClusters(lesser);

      }
      return cluster;
    }

    void FillCluster(Cluster c)
    {
      string bottom = "bottom";
      string top = "top";
      string right = "right";
      string left = "left";

      List<Cell> bottomBorder = new List<Cell>();
      List<Cell> topBorder = new List<Cell>();
      List<Cell> rightBorder = new List<Cell>();
      List<Cell> leftBorder = new List<Cell>();

      for (int i = 0; i < c.size.x; i++)
      {
        Vector2 bottomCellPosition = new Vector2(i, 0) * CellSize + c.OriginPosition;
        Cell bottomCell = grid.GetGridObject(bottomCellPosition);

        bottomBorder.Add(bottomCell);
        for (int j = 0; j < c.size.y; j++)
        {
          Vector2 cellPosition = new Vector2(i, j) * CellSize + c.OriginPosition;
          Cell cell = grid.GetGridObject(cellPosition);
          if (j == c.size.y - 1)
          {
            topBorder.Add(cell);
          }
          if (i == c.size.x - 1)
          {
            rightBorder.Add(cell);
          }
          if (i == 0)
          {
            leftBorder.Add(cell);
          }
        }
      }
      c.borders.Add(bottom, bottomBorder);
      c.borders.Add(left, leftBorder);
      c.borders.Add(right, rightBorder);
      c.borders.Add(top, topBorder);

    }

  }



  private void BuildEntrances(Cluster c1, Cluster c2, KeyValuePair<string, List<Cell>>[] AdjacentBorder)
  {
    Entrance entrance = new Entrance();

    List<Cell> entranceCells = new List<Cell>();
    List<Cell> symmEntranceCells = new List<Cell>();

    KeyValuePair<string, List<Cell>> C1border = AdjacentBorder[0];
    KeyValuePair<string, List<Cell>> C2border = AdjacentBorder[1];

    List<Cell> c1BorderCells = C1border.Value;
    List<Cell> c2BorderCells = C2border.Value;


    for (int i = 0; i < c1BorderCells?.Count; i++)
    {
      Cell cell1 = c1BorderCells[i];
      Cell cell2 = c2BorderCells?[i];
      AddCellsToLists(cell1, cell2);
    }

    if (entranceCells.Count > 0 && symmEntranceCells.Count > 0)
    {
      entrance.FillEntrance(entranceCells);
      entrance.FillSymmEntrance(symmEntranceCells);
      entrance.SetClusters(c1, c2);
    }
    else
    {
      return;
    }
    c1.AddEntrance(entrance);

    string key1 = $"{c1.OriginPosition}->{c2.OriginPosition}";
    setOfEntrances.Add(key1, entrance);

    void AddCellsToLists(Cell cell1, Cell cell2)
    {
      entranceCells.Add(cell1);
      symmEntranceCells.Add(cell2);
    }
  }
  private KeyValuePair<string, List<Cell>>[] GetAdjacentBorder(Cluster c1, Cluster c2)
  {
    Vector2 c1BottomLeft = c1.GridPosition; //BottomLeft
    Vector2 c1TopRight = GetClusterEndGridPosition(c1); //UpperRight
    Vector2 c1BotRight = new Vector2(c1TopRight.x, c1BottomLeft.y);
    Vector2 c1TopLeft = new Vector2(c1BottomLeft.x, c1TopRight.y);

    Vector2 c2BottomLeft = c2.GridPosition; //BottomLeft
    Vector2 c2TopRight = GetClusterEndGridPosition(c2); //UpperRight
    Vector2 c2BotRight = new Vector2(c2TopRight.x, c2BottomLeft.y);
    Vector2 c2TopLeft = new Vector2(c2BottomLeft.x, c2TopRight.y);

    bool C1_ABOVE_C2 = c1BottomLeft == c2TopLeft && c1BotRight == c2TopRight;
    bool C1_LEFT_C2 = c1BotRight == c2BottomLeft && c1TopRight == c2TopLeft;
    bool C1_BELOW_C2 = c1TopLeft == c2BottomLeft && c1TopRight == c2BotRight;
    bool C1_RIGHT_C2 = c1BottomLeft == c2BotRight && c1TopLeft == c2TopRight;

    if (C1_ABOVE_C2)
    {
      KeyValuePair<string, List<Cell>> bottom = new KeyValuePair<string, List<Cell>>("bottom", c1.borders["bottom"]);
      KeyValuePair<string, List<Cell>> top = new KeyValuePair<string, List<Cell>>("top", c2.borders["top"]);

      return new KeyValuePair<string, List<Cell>>[] { bottom, top };
    }
    if (C1_LEFT_C2)
    {
      KeyValuePair<string, List<Cell>> right = new KeyValuePair<string, List<Cell>>("right", c1.borders["right"]);
      KeyValuePair<string, List<Cell>> left = new KeyValuePair<string, List<Cell>>("left", c2.borders["left"]);

      return new KeyValuePair<string, List<Cell>>[] { right, left };
    }
    if (C1_BELOW_C2)
    {
      KeyValuePair<string, List<Cell>> top = new KeyValuePair<string, List<Cell>>("top", c1.borders["top"]);
      KeyValuePair<string, List<Cell>> bottom = new KeyValuePair<string, List<Cell>>("bottom", c2.borders["bottom"]);

      return new KeyValuePair<string, List<Cell>>[] { top, bottom };
    }
    if (C1_RIGHT_C2)
    {
      KeyValuePair<string, List<Cell>> left = new KeyValuePair<string, List<Cell>>("left", c1.borders["left"]);
      KeyValuePair<string, List<Cell>> right = new KeyValuePair<string, List<Cell>>("right", c2.borders["right"]);

      return new KeyValuePair<string, List<Cell>>[] { left, right };
    }
    return default;

    Vector2 GetClusterEndGridPosition(Cluster cluster)
    {
      Vector2 gridPosition = grid.GetGridPosition(cluster.OriginPosition + cluster.WorldSize);
      return gridPosition;
    }
  }
  void BuildGraph()
  {
    foreach (KeyValuePair<string, Entrance> entrance in setOfEntrances)
    {
      List<Cell> cells = entrance.Value.GetEntrance();

      List<Cell> vacant = new List<Cell>();
      List<Cell> full = new List<Cell>();

      int middle = 0;

      for (int i = 0; i < cells.Count; i++)
      {
        Cell c = cells[i];

        if (!c.IsWall)
        {
          vacant.Add(c);
        }
        else
        {
          full.Add(c);
          if (vacant.Count >= 1)
          {
            middle = Mathf.FloorToInt(vacant.Count * 0.5f);
            InstantiateJunction(entrance.Value, vacant[middle]);
            vacant.Clear();
          }
          if (full.Count == cells.Count)
          {
            entrance.Value.isBlocked = true;
          }
        }
      }
      if (vacant.Count > 0)
      {
        middle = Mathf.FloorToInt(vacant.Count * 0.5f);
        int half1 = Mathf.FloorToInt(middle * 0.5f);
        int half2 = Mathf.FloorToInt((middle + vacant.Count) * 0.5f);
        InstantiateJunction(entrance.Value, vacant[middle]);
        InstantiateJunction(entrance.Value, vacant[half1]);
        InstantiateJunction(entrance.Value, vacant[half2]);
      }
    }
    ClusterAddGrid();
    allClustersAllLevels.Add(clustersLevel1);

    for (int i = 0; i < clustersLevel1.GetLength(0); i++)
    {
      for (int j = 0; j < clustersLevel1.GetLength(1); j++)
      {
        Cluster cluster = clustersLevel1[i, j];
        List<Junction> Junctions = cluster.clusterJunctions;
        CalculateEdge(Junctions);
      }
    }


    void InstantiateJunction(Entrance entrance, Cell middleCell)
    {
      if (!middleCell.IsWall)
      {
        Cell symmMiddle = entrance.GetSymmetricalCell(middleCell);

        if (!symmMiddle.IsWall)
        {
          Cluster c1 = entrance.Cluster1;
          Cluster c2 = entrance.Cluster2;

          Junction c1Junction = NewJunction(c1, middleCell);
          Junction c2Junction = NewJunction(c2, symmMiddle);

          c1Junction.SetPair(c2Junction);
          c2Junction.SetPair(c1Junction);

          c1Junction.level = 1;
          c2Junction.level = 1;

          AddJunction(entrance, c1Junction, 1);

          c1.AddJunctionToCluster(c1Junction);
          c2.AddJunctionToCluster(c2Junction);
        }
        else
        {
          entrance.isBlocked = true;
        }
      }
      else
      {
        entrance.isBlocked = true;
      }
      Junction NewJunction(Cluster cluster, Cell CellJunction)
      {
        Junction Junction = new Junction(cluster);
        Junction.SetPosition(CellJunction.WorldPosition);
        Junction.SetGridPosition(CellJunction.GridPosition);
        return Junction;
      }
      void AddJunction(Entrance ent, Junction Junction, int Level)
      {
        Junction.level = Level;
        ent.AddJunction(Junction);
      }
    }
    void ClusterAddGrid()
    {
      for (int i = 0; i < clustersLevel1.GetLength(0); i++)
      {
        for (int j = 0; j < clustersLevel1.GetLength(1); j++)
        {
          Cluster cluster = clustersLevel1[i, j];
          Grid<Cell> clusterGrid = grid.GetFractionOfGrid(cluster.OriginPosition, cluster.size, false);
          cluster.SetGrid(clusterGrid);
        }
      }
    }
  }

  void AddLevelToGraph(int level)
  {
    multiLevelCluster = BuildClusters(level);
    for (int i = 0; i < multiLevelCluster.GetLength(0); i++)
    {
      for (int j = 0; j < multiLevelCluster.GetLength(1); j++)
      {
        Cluster currentCluster = multiLevelCluster[i, j];

        UpdateNeighbours(currentCluster, i, j);

        Grid<Cell> clusterGrid = grid.GetFractionOfGrid(currentCluster.OriginPosition, currentCluster.size, false);
        currentCluster.SetGrid(clusterGrid);
      }

    }
    allClustersAllLevels.Add(multiLevelCluster);


    void UpdateNeighbours(Cluster currentCluster, int i, int j)
    {

      for (int l = -1; l <= 1; l++)
      {
        for (int m = -1; m <= 1; m++)
        {
          //checks only up, down, left and right
          if ((l == 0 && m == 0) || (l == 1 && m == 1) || (l == -1 && m == -1) || (l == -1 && m == 1) || (l == 1 && m == -1))
          {
            continue;
          }
          if ((i + l < 0) || (i + l) >= multiLevelCluster.GetLength(0))
          {
            continue;
          }
          if ((j + m < 0) || (j + m) >= multiLevelCluster.GetLength(1))
          {
            continue;
          }
          Cluster nextCluster = multiLevelCluster[i + l, j + m];
          HandleMultiLevelClusters(currentCluster, nextCluster);
        }
      }
      void HandleMultiLevelClusters(Cluster current, Cluster nextCluster)
      {
        // if (!GetAdjacentBorder(current, nextCluster).Equals(default(KeyValuePair<string, List<string>>)))
        // {
        //  
        // }
        if (GetAdjacentBorder(current, nextCluster).Length > 0)
        {
          UpdateEntrances(current, nextCluster);
        }
      }
      void UpdateEntrances(Cluster cluster, Cluster nextCluster)
      {
        Entrance newEntrance = new Entrance();
        List<Entrance> merge = new List<Entrance>();
        Entrance entrance = null;
        foreach (KeyValuePair<string, Entrance> pair in setOfEntrances)
        {
          entrance = pair.Value;
          if (entrance.HaveEntrance(cluster, nextCluster))
          {
            merge.Add(entrance);
          }
        }
        newEntrance = newEntrance.MergeEntrances(merge.ToArray());
        cluster.AddEntrance(newEntrance);
        UpdateJunctions(cluster, nextCluster);
        void UpdateJunctions(Cluster current, Cluster next)
        {
          foreach (var e in current.entrances)
          {
            foreach (Junction Junction in e.Value.entranceJunctions)
            {
              AddLeveledJunction(current, Junction);
              AddLeveledJunction(next, Junction.Pair);
            }
          }
          List<Junction> Junctions = current.clusterJunctions;
          CalculateEdge(Junctions);

          void AddLeveledJunction(Cluster c, Junction Junction)
          {
            Junction.level = c.level;
            Junction.cluster = c;
            Junction.neighbours.Clear();
            c.AddJunctionToCluster(Junction);
          }
        }
      }
    }
  }

  private static void CalculateEdge(List<Junction> Junctions)
  {
    for (int m = 0; m <= Junctions.Count - 1; m++)
    {
      Junction n1 = Junctions[m];
      n1.AddNeighbour(n1.Pair);
      for (int n = 0; n <= Junctions.Count - 1; n++)
      {
        Junction n2 = Junctions[n];
        n1.AddNeighbour(n2);
      }
    }

  }
  private void DebugGraph()
  {
    if (allClustersAllLevels.Count > 0)
    {
      Color color = new Color();
      foreach (Cluster[,] c in allClustersAllLevels)
      {
        for (int i = 0; i < c.GetLength(0); i++)
        {
          for (int j = 0; j < c.GetLength(1); j++)
          {
            Cluster cluster = c[i, j];
            color = HPA_Utils.ColorByLevel(cluster.level);
            foreach (Junction jun in cluster.clusterJunctions)
            {

              HPA_Utils.DrawCrossInPosition(this, jun.WorldPosition, color);
            }
          }
        }
      }
    }
  }
}

