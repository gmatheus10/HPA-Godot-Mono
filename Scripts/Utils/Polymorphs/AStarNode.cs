using System.Collections.Generic;
using Godot;
public abstract class AStarNode : IComparer<AStarNode>
{
  public int gCost = 0;
  public int hCost = 0;
  public int FCost { get; private set; }
  public AStarNode Parent { get; private set; }
  public Vector2 WorldPosition { get; private set; }
  public Vector2 GridPosition { get; private set; }
  public void SetFCost()
  {
    this.FCost = gCost + hCost;
  }
  public void SetParent(AStarNode Parent)
  {
    this.Parent = Parent;
  }
  public int Compare(AStarNode a, AStarNode b)
  {
    float distance1 = Utils.ManhatamDistance(this.GridPosition, a.GridPosition);
    float distance2 = Utils.ManhatamDistance(this.GridPosition, b.GridPosition);
    return distance1.CompareTo(distance2);
  }
  public void SetGridPosition(Vector2 GridPosition)
  {
    this.GridPosition = GridPosition;
  }
  public void SetPosition(Vector2 WorldPosition)
  {
    this.WorldPosition = WorldPosition;
  }
  public void SetDefault()
  {
    this.gCost = int.MaxValue;
    this.SetFCost();
    this.SetParent(null);
  }
  public override string ToString()
  {
    return ($"{GridPosition.x} - {GridPosition.y} - FCost: {FCost}");
  }
}