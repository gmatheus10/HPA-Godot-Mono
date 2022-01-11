using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public abstract class PositionSender : Node2D
{

 public event EventHandler<EventArgs> PassData;
 public void SendPositions(EventArgs pos)
 {
  PassData?.Invoke(this, pos);
 }
}