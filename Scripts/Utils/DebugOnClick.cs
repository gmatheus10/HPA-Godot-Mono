using Godot;
using System;

public class DebugOnClick : Node2D
{
  Color color1 = new Color();
  private AbstractGraph abstractGraph;
  private Mouse mouse;
  private Vector2 mousePosition;
  private bool start = false;
  public override void _Ready()
  {
    mouse = GetNode<Mouse>("../Mouse");
    abstractGraph = GetNode("../CreateGrid").GetNode<AbstractGraph>("AbstractGraph");
    mouse.MousePositionLeftButton += GetMousePositionLB;
  }

  public void GetMousePositionLB(Vector2 mousePosition)
  {
    start = true;
    this.mousePosition = mousePosition;
    Update();
  }
  public override void _Draw()
  {
    if (start)
    {

      DebugCluster(mousePosition);
    }
  }
  void DebugCluster(Vector2 position)
  {
    foreach (Cluster[,] level in abstractGraph.allClustersAllLevels)
    {
      for (int i = 0; i < level.GetLength(0); i++)
      {
        for (int j = 0; j < level.GetLength(1); j++)
        {
          Cluster cluster = level[i, j];

          if (cluster.IsPositionInside(position))
          {
            HPA_Utils.DrawJunctionsInCluster(this, cluster);
          }
        }
      }
      Update();
    }
  }

}
