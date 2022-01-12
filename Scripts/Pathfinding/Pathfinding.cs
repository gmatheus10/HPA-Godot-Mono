using System.Collections.Generic;
using Godot;
public class Pathfinding
{
  private Grid<Cell> grid;


  public Pathfinding(Grid<Cell> grid)
  {
    this.grid = grid;
  }
  ///  ////////////////////////////////////////////////////////////////////////
  public Grid<Cell> GetGrid()
  {
    return grid;
  }
  public List<Cell> FindPath(Vector2 startWorldPosition, Vector2 endWorldPosition, bool WorldPosition)
  {
    if (WorldPosition)
    {
      Vector2 start = grid.GetGridPosition(startWorldPosition);
      Vector2 end = grid.GetGridPosition(endWorldPosition);
      return FindPath(start, end);
    }

    return FindPath(startWorldPosition, endWorldPosition);

  }

  public List<Cell> FindPath(Vector2 start, Vector2 end)
  {
    Cell startCell = grid.GetGridObject(start, true);
    Cell endCell = grid.GetGridObject(end, true);

    SortedList<float, Cell> openList = new SortedList<float, Cell>();
    HashSet<Cell> closedList = new HashSet<Cell>();

    openList.Add(0, startCell);

    ScanGridAndSetDefault();

    startCell.gCost = 0;
    startCell.hCost = Utils.ManhatamDistance(startCell, endCell);
    startCell.SetFCost();
    // //
    while (openList.Count > 0)
    {
      Cell currentCell = openList.Values[0];
      openList.RemoveAt(0);
      closedList.Add(currentCell);
      foreach (Cell neighbour in GetNeighboursList(currentCell))
      {
        if (neighbour.Equals(endCell))
        {
          neighbour.SetParent(currentCell);
          return CalculatePath(neighbour);
        }

        if (closedList.Contains(neighbour))
        {
          continue;
        }
        int tentativeGCost = currentCell.gCost + Utils.ManhatamDistance(currentCell, neighbour);

        if (tentativeGCost < neighbour.gCost)
        {
          SetNeighbourCellPathValues(neighbour, tentativeGCost);
          IncludeNeighbourOnOpenList(neighbour);
        }
      }
      void SetNeighbourCellPathValues(Cell neighbour, int tentativeGCost)
      {
        neighbour.SetParent(currentCell);
        neighbour.gCost = tentativeGCost;
        neighbour.hCost = Utils.ManhatamDistance(neighbour, endCell);
        neighbour.SetFCost();
      }
      void IncludeNeighbourOnOpenList(Cell neighbour)
      {
        if (neighbour.GridPosition == new Vector2(30, 18))
        {

        }
        if (neighbour.GridPosition == new Vector2(30, 19))
        {

        }
        if (!openList.ContainsKey(neighbour.FCost))
        {
          openList.Add(neighbour.FCost, neighbour);
        }
        else
        {
          Cell insider = openList[neighbour.FCost];
          if (insider.gCost > neighbour.gCost)
          {
            //if the gCost is higher, it means that the hCost is lower
            openList.Remove(neighbour.FCost);
            openList.Add(neighbour.FCost, neighbour);
            // insider.SetParent(null);
          }
          // else
          // {
          //   neighbour.SetParent(null);
          // }
        }
      }
    }
    // out of Cell on the open list
    GD.Print($"Couldn't find the path between cells {startCell} and {endCell}");


    return null;
    void ScanGridAndSetDefault()
    {
      for (int x = 0; x < (int)grid.GridSize.x - 1; x++)
      {
        for (int y = 0; y < (int)grid.GridSize.y - 1; y++)
        {
          grid.GridArray[x, y].SetDefault();
        }
      }
    }
    List<Cell> CalculatePath(Cell goal)
    {
      List<Cell> path = new List<Cell>();
      path.Add(goal);
      Cell queue = goal;
      while (queue.Parent != null)
      {
        path.Add((Cell)queue.Parent);
        queue = (Cell)queue.Parent;
      }
      path.Reverse();
      return path;
    }
  }

  private List<Cell> GetNeighboursList(Cell currentCell)
  {
    float centerX = currentCell.WorldPosition.x;
    float centerY = currentCell.WorldPosition.y;

    float leftPosX = currentCell.WorldPosition.x - grid.CellSize;
    float rightPosX = currentCell.WorldPosition.x + grid.CellSize;
    float upPosY = currentCell.WorldPosition.y + grid.CellSize;
    float downPosY = currentCell.WorldPosition.y - grid.CellSize;

    List<Cell> neighbours = new List<Cell>();

    try
    {
      if (leftPosX >= grid.OriginPosition.x)
      {
        Cell leftCell = grid.GetGridObject(new Vector2(leftPosX, centerY));
        AddNonWall(leftCell);

        if (downPosY >= grid.OriginPosition.y)
        {
          Cell downLeftCell = grid.GetGridObject(new Vector2(leftPosX, downPosY));
          AddNonWall(downLeftCell);
        }

        if (upPosY < grid.FinalPosition.y)
        {
          Cell upLeftCell = grid.GetGridObject(new Vector2(leftPosX, upPosY));
          AddNonWall(upLeftCell);
        }

      }
      if (rightPosX < grid.FinalPosition.x)
      {
        Cell rightCell = grid.GetGridObject(new Vector2(rightPosX, centerY));
        AddNonWall(rightCell);

        if (downPosY >= grid.OriginPosition.y)
        {
          Cell rightDownCell = grid.GetGridObject(new Vector2(rightPosX, downPosY));
          AddNonWall(rightDownCell);
        }

        if (upPosY < grid.FinalPosition.y)
        {
          Cell rightUpCell = grid.GetGridObject(new Vector2(rightPosX, upPosY));
          AddNonWall(rightUpCell);
        }

      }
      if (downPosY >= grid.OriginPosition.y)
      {
        Cell downCell = grid.GetGridObject(new Vector2(centerX, downPosY));
        AddNonWall(downCell);
      }
      if (upPosY < grid.FinalPosition.y)
      {
        Cell upCell = grid.GetGridObject(new Vector2(centerX, upPosY));
        AddNonWall(upCell);
      }

    }
    catch (System.Exception)
    {
      throw;
    }
    return neighbours;

    void AddNonWall(Cell cell)
    {
      try
      {
        if (!cell.IsWall)
        {
          neighbours.Add(cell);
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}
