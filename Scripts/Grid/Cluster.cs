using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
[Serializable]
public class Cluster
{
  public int level;
  public Vector2 size;
  public Vector2 WorldSize { get; private set; }
  public Vector2 OriginPosition { get; private set; }
  public Vector2 GridPosition { get; private set; }
  public Dictionary<string, List<Cell>> borders = new Dictionary<string, List<Cell>>();
  public Dictionary<string, Entrance> entrances = new Dictionary<string, Entrance>();
  public List<Junction> clusterJunctions = new List<Junction>();
  public List<Cluster> lesserLevelClusters = new List<Cluster>();
  public Grid<Cell> ClusterGrid { get; private set; }
  public Cluster(Vector2 size, Vector2 originPosition, int level = 1)
  {
    this.OriginPosition = originPosition;
    this.level = level;
    this.size = size;
  }

  public void SetGridPosition(Vector2 gridPosition)
  {
    this.GridPosition = gridPosition;
  }
  public void SetWorldSize(Vector2 WorldSize)
  {
    this.WorldSize = WorldSize;
  }
  public void AddEntrance(Entrance entrance)
  {
    string key1 = $"{entrance.Cluster1.OriginPosition}->{entrance.Cluster2.OriginPosition}";
    entrances.Add(key1, entrance);
  }
  public void AddJunctionToCluster(Junction Junction)
  {
    if (this.IsPositionInside(Junction.WorldPosition))
    {
      clusterJunctions.Add(Junction);
    }

  }
  public void RemoveJunctionFromCluster(Junction Junction)
  {
    if (clusterJunctions.Contains(Junction))
    {
      clusterJunctions.Remove(Junction);
    }
  }

  public bool IsPositionInside(Vector2 position)
  {
    Vector2 thisStart = this.OriginPosition;
    Vector2 thisEnd = thisStart + WorldSize;


    bool xLargerStart = position.x >= thisStart.x;
    bool yLargerStart = position.y >= thisStart.y;

    bool xSmallerEnd = position.x <= thisEnd.x;
    bool ySmallerEnd = position.y <= thisEnd.y;

    return xLargerStart && xSmallerEnd && yLargerStart && ySmallerEnd;
  }
  private bool IsClusterInside(Cluster cluster)
  {
    bool isStartinside = this.IsPositionInside(cluster.OriginPosition);
    bool isEndinside = this.IsPositionInside(cluster.OriginPosition + new Vector2(cluster.size.x, cluster.size.y));

    return isStartinside && isEndinside;
  }
  public bool IsEntranceInside(List<Cell> entrance)
  {
    int count = 0;
    foreach (Cell cell in entrance)
    {
      if (IsPositionInside(cell.WorldPosition))
      {
        count++;
      }
    }
    if (count == entrance.Count)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
  public void AddLesserClusters(Cluster[,] lesserClustersArray)
  {
    //need a list / array of the lesserClusters 
    //detect clusters inside
    //IsPositionInside for cluster origin position and for cluster end position
    for (int i = 0; i < lesserClustersArray.GetLength(0); i++)
    {
      for (int j = 0; j < lesserClustersArray.GetLength(1); j++)
      {
        Cluster cluster = lesserClustersArray[i, j];
        if (IsClusterInside(cluster))
        {
          lesserLevelClusters.Add(cluster);

        }
      }
    }

    //sort them on the array
  }
  public void SetGrid(Grid<Cell> clusterGrid)
  {
    this.ClusterGrid = clusterGrid;
  }


}
