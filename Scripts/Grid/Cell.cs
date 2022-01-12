using System.Collections;
using System.Collections.Generic;
using System;
using Godot;

[Serializable]
public class Cell : AStarNode, IEquatable<Cell>
{
  public bool IsWall { get; private set; }
  [NonSerialized] private readonly Grid<Cell> grid;
  public Cell(Grid<Cell> grid, int x, int y)
  {
    this.grid = grid;
    Vector2 centerTranslocation = new Vector2(grid.CellSize, grid.CellSize) * 0.5f;
    SetPosition(grid.GetWorldPosition(x, y) + centerTranslocation);

    SetGridPosition(new Vector2(x, y));
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

}
