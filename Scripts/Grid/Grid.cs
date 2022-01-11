
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
// [Serializable]
public class Grid<T>
{
  public bool ShowGrid;
  public int Width { get; private set; }
  public int Height { get; private set; }
  public Vector2 GridSize { get; private set; }
  public T[,] GridArray { get; private set; }

  public float CellSize { get; private set; }
  public Vector2 OriginPosition { get; private set; }
  public Vector2 FinalPosition { get; private set; }

  public Grid() { }
  public Grid(int width, int height, float cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createGridObject, bool showGrid)
  {
    this.CellSize = cellSize;
    this.OriginPosition = originPosition;
    this.Width = (int)(width * cellSize);
    this.Height = (int)(height * cellSize);
    GridArray = new T[width, height];

    FinalPosition = GetWorldPosition(width, height);
    GridSize = new Vector2(Width, Height) / cellSize;


    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        T gridObject = createGridObject(this, x, y);

        SetGridObject(x, y, gridObject);
      }
    }
  }
  public Grid(Vector2 size, float cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createGridObject, bool showGrid) : this((int)size.x, (int)size.y, cellSize, originPosition, createGridObject, showGrid)
  {

  }
  public Vector2 GetWorldPosition(int x, int y)
  {
    Vector2 newVec = new Vector2(x, y) * CellSize + OriginPosition;
    return newVec;
  }
  public Vector2 GetGridPosition(Vector2 position)
  {
    float porcX;
    float porcY;

    Vector2 positionOnGrid = position - OriginPosition;
    //Debug.Log( "Position on Grid:" + " " + positionOnGrid );
    porcX = (Mathf.Clamp(positionOnGrid.x / Width, 0, 1));
    porcY = (Mathf.Clamp(positionOnGrid.y / Height, 0, 1));
    int gridX = Mathf.FloorToInt(porcX * GridSize.x);
    int gridY = Mathf.FloorToInt(porcY * GridSize.y);
    //Debug.Log( $"{gridX} / {gridY}" );
    return new Vector2(gridX, gridY);
  }


  public Vector2 GetGridPosition(int x, int y)
  {
    return GetGridPosition(new Vector2(x, y));
  }
  public T GetGridObject(Vector2 position, bool isGridPosition = false)
  {
    //Debug.Log( position );
    if (isGridPosition == true)
    {
      return GetGridObject((int)position.x, (int)position.y);
    }
    Vector2 gridPosition = GetGridPosition(position);
    return GetGridObject((int)gridPosition.x, (int)gridPosition.y);
  }

  public T GetGridObject(int x, int y)
  {
    try
    {
      T gridObject = GridArray[x, y];
      return gridObject;
    }
    catch (Exception)
    {
      return default(T);
    }
  }
  public void SetGridObject(int x, int y, T value)
  {
    GridArray[x, y] = value;

  }
  public void SetGridObject(Vector2 worldPosition, T value)
  {
    Vector2 gridPositions = GetGridPosition(worldPosition);
    SetGridObject((int)gridPositions.x, (int)gridPositions.y, value);
  }
  public bool IsInsideGrid(int x, int y)
  {
    return (x >= 0 && y >= 0 && x < Width && y < Height);
  }
  public bool IsInsideGrid(Vector2 position)
  {
    bool greaterX = position.x >= OriginPosition.x;
    bool smallerX = position.x < FinalPosition.x;
    bool greaterY = position.y >= OriginPosition.y;
    bool smallerY = position.y < FinalPosition.y;
    bool isInside = greaterX && smallerX && greaterY && smallerY;
    return isInside;
  }
  public Grid<T> GetFractionOfGrid(Vector2 startPosition, Vector2 size, bool showGrid)
  {
    Vector2 fracStart = GetGridPosition(startPosition);
    Grid<T> frac = new Grid<T>(size, this.CellSize, startPosition, (Grid<T> g, int x, int y) => GridArray[(int)(x + fracStart.x), (int)(y + fracStart.y)], showGrid);

    return frac;
  }

}
