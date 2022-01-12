using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
public class HierarchicalPathfinding : Node2D
{
  //Recieves the Junction from PlayerController and converts into a path list
  // Give the path list to the Movement.cs
  private AbstractGraph abstractGraph;
  int MaxLevel;
  //Events publishers scripts
  private PositionSender positionSender;
  Junction endJunction = null;
  Junction startJunction = null;
  public List<KeyValuePair<int, List<Junction>>> RefinedPath { get; private set; }
  public delegate void PassHierarchy(List<Junction> HierarchicalPath);
  public event PassHierarchy OnHierarchyComplete;

  public override void _Ready()

  {
    GetScripts();
    Start();
  }
  private void GetScripts()
  {
    abstractGraph = GetNode("../../../CreateGrid").GetNode<AbstractGraph>("AbstractGraph");
    positionSender = GetNode<PositionSender>("../../PlayerController");
  }
  private void Start()
  {
    positionSender.PassData += PlayerController_DestinationRecieved;
    MaxLevel = abstractGraph.Level;
  }
  private void PlayerController_DestinationRecieved(object sender, EventArgs pos)
  {
    JunctionArgs junctions = (JunctionArgs)pos;
    if (this.endJunction?.GridPosition != junctions.EndJunction.GridPosition)
    {
      SetJunctions(((JunctionArgs)pos).StartJunction, ((JunctionArgs)pos).EndJunction);
      FindPath();
      List<Junction> Level1Path = GetLevelOnePaths(RefinedPath);
      if (Level1Path.Count > 0)
      {
        OnHierarchyComplete?.Invoke(Level1Path);
      }
    }
  }
  private void SetJunctions(Junction start, Junction end)
  {
    this.startJunction = start;
    this.endJunction = end;
  }
  private void FindPath()
  {
    List<Junction> abstractPath = HierarchicalSearch(startJunction, endJunction, MaxLevel);
    RefinedPath = RefinePath(abstractPath, MaxLevel);
  }
  private List<Junction> GetLevelOnePaths(List<KeyValuePair<int, List<Junction>>> RefinedPath)
  {
    List<Junction> Level1 = new List<Junction>();
    foreach (var pair in RefinedPath)
    {
      if (pair.Key == 1)
      {
        Level1.AddRange(pair.Value);
      }
    }
    return Level1;
  }
  private List<Junction> HierarchicalSearch(Junction start, Junction end, int Level)
  {
    if (start == null || end == null)
    {
      return null;
    }
    if (start.WorldPosition == end.WorldPosition)
    {
      return new List<Junction>() { start };
    }
    InsertJunction(start, Level);
    InsertJunction(end, Level);

    List<Junction> abstractPath = SearchForPath(start, end, Level);
    RemoveJunction(start, Level);
    RemoveJunction(end, Level);
    return abstractPath;

    void InsertJunction(Junction Junction, int maxLevel)
    {

      for (int i = 1; i <= maxLevel; i++)
      {
        Cluster c = GetCluster(Junction.WorldPosition, i);
        Junction.cluster = c;
        ConnectToBorder(Junction, c);
      }
      Junction.level = maxLevel;
      Junction.neighbours.Sort(Junction);

      void ConnectToBorder(Junction junction, Cluster cluster)
      {
        int level = cluster.level;
        List<Junction> clusterJunctions = cluster.clusterJunctions;

        foreach (Junction n in clusterJunctions)
        {
          if (n.level < level)
          {
            continue;
          }
          junction.AddNeighbour(n);
          n.AddNeighbour(Junction);
        }
        cluster.AddJunctionToCluster(Junction);
      }
    }
    void RemoveJunction(Junction Junction, int maxLevel)
    {
      for (int i = 1; i <= maxLevel; i++)
      {
        Cluster c = GetCluster(Junction.WorldPosition, i);
        c.RemoveJunctionFromCluster(Junction);
        Disconnect(Junction);
      }
      void Disconnect(Junction junction)
      {
        foreach (Junction neig in junction.neighbours)
        {
          neig.neighbours.Remove(junction);
        }
      }
    }
  }

  List<KeyValuePair<int, List<Junction>>> RefinePath(List<Junction> Path, int Level)
  {
    if (Path == null)
    {
      return null;
    }
    List<KeyValuePair<int, List<Junction>>> paths = new List<KeyValuePair<int, List<Junction>>>();
    paths.Add(new KeyValuePair<int, List<Junction>>(Level, Path));
    Hierarchical(Path, Level);
    return paths;
    //To do:  return a list of the lowest level path
    void Hierarchical(List<Junction> path, int level)
    {
      level--;
      if (level < 1)
      {
        return;
      }
      List<Junction> lesserPath = new List<Junction>();
      for (int i = 1; i < path.Count; i++)
      {
        Cluster iCluster = GetCluster(path[i].WorldPosition, level + 1);
        Cluster iMinusCluster = GetCluster(path[i - 1].WorldPosition, level + 1);
        if (iCluster != iMinusCluster)
        {
          continue;
        }


        Junction ephemeralStart = CopyJunction(path[i - 1]);
        Junction ephemeralEnd = CopyJunction(path[i]);

        List<Junction> less = HierarchicalSearch(ephemeralStart, ephemeralEnd, level);
        lesserPath.AddRange(less);



        ephemeralStart = null;
        ephemeralEnd = null;
      }
      paths.Add(new KeyValuePair<int, List<Junction>>(level, lesserPath));
      Hierarchical(lesserPath, level);

    }
    Junction CopyJunction(Junction Junction)
    {
      Junction copy = new Junction();
      copy.SetPosition(Junction.WorldPosition);
      copy.SetGridPosition(Junction.GridPosition);
      return copy;
    }

  }
  //Helpers
  private List<Junction> SearchForPath(Junction startJunction, Junction endJunction, int Level)
  {
    SortedList<float, Junction> openList = new SortedList<float, Junction>();
    List<Junction> closedList = new List<Junction>();
    ScanGridAndSetDefault();

    startJunction.gCost = 0;
    startJunction.hCost = Utils.ManhatamDistance(startJunction.GridPosition, endJunction.GridPosition);
    startJunction.SetFCost();

    openList.Add(startJunction.FCost, startJunction);

    while (openList.Count > 0)
    {
      Junction currentJunction = openList.Values[0];

      openList.RemoveAt(0);
      closedList.Add(currentJunction);

      foreach (Junction neighbour in currentJunction.neighbours)
      {
        if (neighbour.level < Level)
        {
          continue;
        }
        if (closedList.Contains(neighbour))
        {
          continue;
        }
        if (neighbour.Equals(endJunction))
        {
          neighbour.SetParent(currentJunction);
          return CalculatePath(neighbour);
        }


        int newG = currentJunction.gCost + Utils.ManhatamDistance(neighbour.GridPosition, currentJunction.GridPosition);
        if (newG < neighbour.gCost)
        {
          ConfigureNeighbour(endJunction, currentJunction, neighbour, newG);

          if (!openList.ContainsKey(neighbour.FCost))
          {
            openList.Add(neighbour.FCost, neighbour);
          }
          else
          {
            Junction insider = openList[neighbour.FCost];
            if (insider.gCost < neighbour.gCost)
            {
              openList.Remove(neighbour.FCost);
              openList.Add(neighbour.FCost, neighbour);
            }
          }
        }
      }

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    GD.Print($"Didn't find it - start Junction: {startJunction.GridPosition} to end Junction: {endJunction.GridPosition} on Level: {Level}");

    return null;
    void ScanGridAndSetDefault()
    {
      List<Cluster[,]> allClusters = abstractGraph.allClustersAllLevels;

      foreach (Cluster[,] setOfClusters in allClusters)
      {
        for (int i = 0; i < setOfClusters.GetLength(0); i++)
        {
          for (int j = 0; j < setOfClusters.GetLength(1); j++)
          {
            Cluster current = setOfClusters[i, j];

            for (int m = 0; m <= current.clusterJunctions.Count - 1; m++)
            {
              Junction Junction = current.clusterJunctions[m];
              Junction.gCost = int.MaxValue;
              Junction.SetFCost();
              Junction.SetParent(null);
            }

          }
        }
      }
    }
    void ConfigureNeighbour(Junction EndJunction, Junction currentJunction, Junction neighbour, int newG)
    {
      neighbour.SetParent(currentJunction);
      neighbour.gCost = newG;
      neighbour.hCost = Utils.ManhatamDistance(neighbour.GridPosition, EndJunction.GridPosition);
      neighbour.SetFCost();
    }
    List<Junction> CalculatePath(Junction EndJunction)
    {
      List<Junction> path = new List<Junction>();

      path.Add(EndJunction);
      Junction queue = EndJunction;
      while (queue.Parent != null)
      {
        path.Add((Junction)queue.Parent);
        queue = (Junction)queue.Parent;
      }
      path.Reverse();
      return path;
    }
  }
  private Cluster GetCluster(Vector2 position, int Level)
  {
    List<Cluster[,]> allClusters = abstractGraph.allClustersAllLevels;
    foreach (Cluster[,] setOfClusters in allClusters)
    {
      for (int i = 0; i < setOfClusters.GetLength(0); i++)
      {
        for (int j = 0; j < setOfClusters.GetLength(1); j++)
        {
          Cluster current = setOfClusters[i, j];
          if (current.IsPositionInside(position))
          {
            if (current.level == Level)
            {
              return current;
            }
          }
        }
      }
    }
    return null;
  }

}
