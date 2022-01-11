using Godot;
using System.Collections.Generic;

public class PathfindingConfig : Node2D
{
  [Export]
  public int MovementSpeed;
  Pathfinding pathFinding;
  HierarchicalPathfinding hierarchical;
  Movement movement;
  List<Junction> hierarchicalPath;
  List<Cell> pathCells = new List<Cell>();
  public delegate void SendAStar(List<Cell> AStar);
  public event SendAStar OnAStarFound;

  private int pathIndex;
  public override void _Ready()
  {
    hierarchical = GetNode<HierarchicalPathfinding>("HierarchicalPathfinding");
    hierarchical.OnHierarchyComplete += GetPath;
    movement = GetNode<Movement>("Movement");
    movement.OnPathReached += UpdatePath;
  }
  private void UpdatePath()
  {
    pathIndex++;
    if (pathIndex + 1 < hierarchicalPath.Count)
    {

      FindPath(hierarchicalPath);
    }
  }

  private void GetPath(List<Junction> hierarchical)
  {
    this.hierarchicalPath = hierarchical;
    Reset();
    FindPath(hierarchicalPath);
  }
  private void Reset()
  {
    pathCells.Clear();
    pathIndex = 0;
  }
  private void FindPath(List<Junction> currentHierarchical)
  {
    Junction current = currentHierarchical[pathIndex];
    Junction next = currentHierarchical[pathIndex + 1];

    if (current.cluster == next.cluster)
    {
      pathFinding = new Pathfinding(current.cluster.ClusterGrid);

      List<Cell> path = pathFinding.FindPath(current.WorldPosition, next.WorldPosition, true);
      pathCells = path;
    }
    else
    {

      List<Cell> diffGrid = new List<Cell>() { current.GetCell(), next.GetCell() };
      pathCells = diffGrid;
    }
    OnAStarFound?.Invoke(pathCells);

  }
}
