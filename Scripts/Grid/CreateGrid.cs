
using System;
using Godot;
public class CreateGrid : Node2D
{
  private AbstractGraph abstractGraph;
  [Export]
  public int CellSize;
  public Grid<Cell> Grid { get; private set; }
  [Export]
  public Vector2 GridSize;
  [Export]
  public int Level;
  [Export]
  public Vector2 LevelOneClusterSize;

  // Start is called before the first frame update

  public override void _Ready()
  {
    InstantiateNewGrid();
    CreateAbstractGraph();
  }
  private void CreateAbstractGraph()
  {
    abstractGraph = GetNode<AbstractGraph>("AbstractGraph");
    abstractGraph.Start(this.Grid, this.LevelOneClusterSize, this.Level);
    abstractGraph.PreProcessing(this.Level);
  }
  private void InstantiateNewGrid()
  {

    int GridSizeX = Mathf.RoundToInt(GridSize.x);
    int GridSizeY = Mathf.RoundToInt(GridSize.y);

    Vector2 originPosition = Position;
    Grid = new Grid<Cell>(GridSizeX, GridSizeY, CellSize, originPosition, InstatiateCell, true);

  }

  private Cell InstatiateCell(Grid<Cell> g, int x, int y)
  {
    return new Cell(g, x, y);
  }
  public override void _Draw()
  {
    Vector2 originPosition = Position;
    Vector2 LEFT_UP = originPosition + new Vector2(0, GridSize.y) * CellSize;
    Vector2 RIGHT_UP = originPosition + new Vector2(GridSize.x, GridSize.y) * CellSize;
    Vector2 RIGHT_DOWN = originPosition + new Vector2(GridSize.x, 0) * CellSize;

    DrawGrid();

  }
  private void DrawGrid()
  {
    Color color = new Color("#cccccc");
    if (Grid != null)
    {
      for (int x = 0; x < GridSize.x; x++)
      {
        for (int y = 0; y < GridSize.y; y++)
        {
          DrawLine(Grid.GetWorldPosition(x, y), Grid.GetWorldPosition(x, y + 1), color);
          DrawLine(Grid.GetWorldPosition(x, y), Grid.GetWorldPosition(x + 1, y), color);

        }
      }
      DrawLine(Grid.GetWorldPosition(0, Grid.Height), Grid.GetWorldPosition(Grid.Width, Grid.Height), color);
      DrawLine(Grid.GetWorldPosition(Grid.Width, 0), Grid.GetWorldPosition(Grid.Width, Grid.Height), color);

    }
  }
}
