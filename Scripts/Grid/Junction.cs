using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
[Serializable]
public class Junction : AStarNode, IEquatable<Junction>
{
  public int level;
  public Junction Pair { get; private set; }
  public Cluster cluster;
  public List<Junction> neighbours = new List<Junction>();

  public Junction()
  {
  }
  public Junction(Cluster cluster)
  {
    this.cluster = cluster;
  }

  public void SetPair(Junction pair)
  {
    this.Pair = pair;
    neighbours.Add(pair);
  }
  public void AddNeighbour(Junction Junction)
  {
    if (this.WorldPosition != Junction.WorldPosition)
    {
      if (!this.neighbours.Contains(Junction))
      {
        this.neighbours.Add(Junction);
      }
    }
  }
  public bool Equals(Junction other)
  {
    if (other == null)
    {
      return false;
    }
    if (this.WorldPosition == other.WorldPosition && this.GridPosition == other.GridPosition && this.level == other.level)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
  public Cell GetCell()
  {
    return this.cluster.ClusterGrid.GetGridObject(this.WorldPosition);
  }
}

