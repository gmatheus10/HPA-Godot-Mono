
using System.Collections;
using System.Collections.Generic;
using Godot;

public static class HPA_Utils
{
  public static Color color = new Color();
  private static Vector2 CanvasTransform(CanvasItem obj, Vector2 position)
  {
    return obj.GetGlobalTransform().XformInv(position);
  }
  public static void DrawCrossInPosition(CanvasItem obj, Vector2 position, Color color)
  {

    float crossLength = 8f;
    Vector2 crossX = new Vector2(crossLength, 0);
    Vector2 crossY = new Vector2(0, crossLength);
    obj.DrawLine(CanvasTransform(obj, position) - crossX, CanvasTransform(obj, position) + crossX, color);
    obj.DrawLine(CanvasTransform(obj, position) - crossY, CanvasTransform(obj, position) + crossY, color);
  }
  public static void DrawJunctionsInCluster(CanvasItem obj, Cluster cluster)
  {
    foreach (Junction Junction in cluster.clusterJunctions)
    {
      DrawCrossInPosition(obj, Junction.WorldPosition, color.yellow());
    }
  }
  public static void ShowPathLines(CanvasItem obj, List<Vector2> path, Color color)
  {
    if (path != null)
    {
      for (int i = 1; i <= path.Count - 1; i++)
      {
        obj.DrawLine(CanvasTransform(obj, path[i]), CanvasTransform(obj, path[i - 1]), color);
      }
    }
  }
  public static void ShowPathLines(CanvasItem obj, List<Junction> path, Color color)
  {
    if (path != null)
    {
      List<Vector2> newPath = new List<Vector2>();
      foreach (Junction Junction in path)
      {
        newPath.Add(Junction.WorldPosition);
        DrawCrossInPosition(obj, Junction.WorldPosition, color);
      }
      ShowPathLines(obj, newPath, color);
    }
  }
  public static void ShowPathLines(CanvasItem obj, List<Cell> path, Color color)
  {
    if (path != null)
    {
      List<Vector2> newPath = new List<Vector2>();
      foreach (Cell cell in path)
      {
        newPath.Add(cell.WorldPosition);
      }
      ShowPathLines(obj, newPath, color);
    }
  }
  public static void DebugRefinedPath(CanvasItem obj, List<KeyValuePair<int, List<Junction>>> RefinedPath)
  {
    if (RefinedPath == null)
    {
      return;
    }
    int level = 0;

    foreach (var pair in RefinedPath)
    {
      level = pair.Key;
      List<Junction> path = pair.Value;
      ShowPathLines(obj, path, ColorByLevel(level));
    }

  }
  public static Color ColorByLevel(int level)
  {
    Color color = new Color();
    switch (level)
    {
      case 1:
        color = color.blue();
        break;
      case 2:
        color = color.yellow();
        break;
      case 3:
        color = color.green();
        break;
      default:
        color = color.white();
        break;
    }
    return color;
  }
}
