using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
[Serializable]
public class Junction : IComparer<Junction>, IEquatable<Junction>
{
 public int level;
 public float gCost = 0;
 public float hCost = 0;
 public float FCost { get; private set; }
 public Vector2 WorldPosition { get; private set; }
 public Vector2 GridPosition { get; private set; }
 public Junction Pair { get; private set; }
 public Junction Parent { get; private set; }
 public Cluster cluster;
 public List<Junction> neighbours = new List<Junction>();

 public Junction()
 {
 }
 public Junction(Cluster cluster)
 {
  this.cluster = cluster;
 }

 public void SetPosition(Vector2 WorldPosition)
 {
  this.WorldPosition = WorldPosition;
 }
 public void SetGridPosition(Vector2 GridPosition)
 {
  this.GridPosition = GridPosition;
 }
 public void SetFCost()
 {
  this.FCost = gCost + hCost;
 }
 public void SetParent(Junction Parent)
 {
  this.Parent = Parent;
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
 public int Compare(Junction a, Junction b)
 {
  float distance1 = Utils.ManhatamDistance(this.GridPosition, a.GridPosition);
  float distance2 = Utils.ManhatamDistance(this.GridPosition, b.GridPosition);
  return distance1.CompareTo(distance2);
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

