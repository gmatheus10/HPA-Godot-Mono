using Godot;
using System;

public class GridConfiguration : Resource
{
 [Export]
 public int CellSize { get; set; }
 [Export]
 public Vector2 GridSize { get; set; }
 [Export]
 public int Level { get; set; }
 [Export]
 public Vector2 LevelOneClusterSize { get; set; }
 public GridConfiguration()
 {

 }
 public GridConfiguration(Vector2 GridSize, Vector2 LevelOneClusterSize, int Level = 1, int CellSize = 32)
 {
  this.GridSize = GridSize;
  this.LevelOneClusterSize = LevelOneClusterSize;
  this.Level = Level;
  this.CellSize = CellSize;
 }

}
