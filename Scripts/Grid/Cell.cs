using System.Collections;
using System.Collections.Generic;
using System;
using Godot;

[Serializable]
public class Cell : IEquatable<Cell>
{
  public int gCost = 0;
  public int hCost = 0;
  private int fCost = 0;
  public int FCost { get { return fCost; } }

  public Vector2 WorldPosition { get; private set; }
  public Vector2 GridPosition { get; private set; }
  public bool IsWall { get; private set; }

  public Cell cameFromCell { get; set; }

  [NonSerialized] private readonly Grid<Cell> grid;
  public Cell(Grid<Cell> grid, int x, int y)
  {
    this.grid = grid;
    Vector2 centerTranslocation = new Vector2(grid.CellSize, grid.CellSize) * 0.5f;
    WorldPosition = grid.GetWorldPosition(x, y) + centerTranslocation;

    GridPosition = new Vector2(x, y);
  }
  public override string ToString()
  {
    return ($"{GridPosition.x} - {GridPosition.y}");
  }
  public void CalculateF()
  {
    fCost = gCost + hCost;
  }
  public bool Equals(Cell other)
  {
    if (other == null)
    {
      return false;
    }
    if (other.WorldPosition == this.WorldPosition && other.GridPosition == this.GridPosition)
    {
      return true;
    }
    return false;
  }
  public void SetDefaultCell()
  {
    this.gCost = int.MaxValue;
    this.CalculateF();
    this.cameFromCell = null;
  }
}
