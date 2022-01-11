using Godot;
using System;
using System.Collections.Generic;
public class DrawGrid : Node2D
{
 private AbstractGraph abstractGraph;
 private Vector2 LevelOneClusterSize;
 private List<Cluster[,]> allClustersAllLevels;
 public override void _Ready()
 {
  abstractGraph = GetParent<AbstractGraph>() as AbstractGraph;
  this.LevelOneClusterSize = abstractGraph.LevelOneClusterSize;
  this.allClustersAllLevels = abstractGraph.allClustersAllLevels;
 }
 public override void _Draw()
 {
  foreach (Cluster[,] clusters in allClustersAllLevels)
  {
   for (int i = 0; i < clusters.GetLength(0); i++)
   {
    for (int j = 0; j < clusters.GetLength(1); j++)
    {
     Cluster c = clusters[i, j];
     Rect2 rect = new Rect2(c.OriginPosition, c.WorldSize);
     Color color = new Color();
     if (c.level == 1)
     {
      color = new Color("#0000FF");
     }
     if (c.level == 2)
     {
      color = new Color("#FFFF00");
     }
     if (c.level == 3)
     {
      color = new Color("#00FF00");
     }
     DrawRect(rect, color, false);
    }
   }
  }
 }
}
