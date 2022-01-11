using System;
using Godot;
public class PositionsArgs : EventArgs
{
 
 public Vector2 StartPosition { get; private set; }
 public Vector2 EndPosition { get; private set; }

 public PositionsArgs(Vector2 StartPosition, Vector2 EndPosition)
 {
  this.StartPosition = StartPosition;
  this.EndPosition = EndPosition;
 }
}