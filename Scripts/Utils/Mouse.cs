using Godot;
using System;

public class Mouse : Node2D
{
 public delegate void OnMouseRightClick(Vector2 mousePosition);
 public delegate void OnMouseLeftClick(Vector2 mousePosition);
 public event OnMouseLeftClick MousePositionLeftButton;
 public event OnMouseRightClick MousePositionRightButton;
 public override void _UnhandledInput(InputEvent @event)
 {
  if (@event is InputEventMouseButton)
  {
   InputEventMouseButton emb = (InputEventMouseButton)@event;
   if (emb.IsPressed())
   {
    if (emb.ButtonIndex == (int)ButtonList.Left)
    {
     MousePositionLeftButton?.Invoke(GetGlobalMousePosition());
    }
    if (emb.ButtonIndex == (int)ButtonList.Right)
    {
     MousePositionRightButton?.Invoke(GetGlobalMousePosition());
    }
   }
  }
 }
}
