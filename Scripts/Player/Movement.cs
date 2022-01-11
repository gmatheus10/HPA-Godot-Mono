using Godot;
using System;
using System.Collections.Generic;

public class Movement : Node2D
{
  public delegate void ContinuePath();
  public event ContinuePath OnPathReached;
  public PathfindingConfig pfConfig;
  private KinematicBody2D body;

  private int speed;
  private Vector2 target = new Vector2();
  private Vector2 velocity = new Vector2();
  private int currentIndex;
  private List<Cell> path = new List<Cell>();
  private bool allowDraw = false;
  public override void _Ready()
  {
    pfConfig = GetNode<PathfindingConfig>("../../Pathfinding");
    pfConfig.OnAStarFound += PathRecieved;
    speed = pfConfig.MovementSpeed;
    body = GetNode("../../Pathfinding").GetParent<KinematicBody2D>();
  }
  private void PathRecieved(List<Cell> AStar)
  {
    path.Clear();
    path = AStar;
    currentIndex = 1;
    allowDraw = true;
  }
  public override void _Draw()
  {
    if (allowDraw)
    {

      HPA_Utils.DrawCrossInPosition(this, target, HPA_Utils.color.yellow());
    }
  }
  public override void _Process(float delta)
  {
    Move(path);
  }
  private void Move(List<Cell> currentPath)
  {
    if (currentPath?.Count > 0)
    {

      if (currentPath != path)
      {
        GD.Print("here2");
        return;
      }
      if (currentIndex < currentPath.Count)
      {
        target = currentPath[currentIndex].WorldPosition;
        Update();

        velocity = body.Position.DirectionTo(target) * speed;
        if (body.Position.DistanceTo(target) > 5)
        {
          velocity = body.MoveAndSlide(velocity);
        }
        else
        {
          body.Position = target;
        }
        if (body.Position == target)
        {
          currentIndex++;
        }
      }
      else
      {
        OnPathReached?.Invoke();
        //invoke event to get the next path list
      }

    }


  }
  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("ui_accept"))
    {
      OnPathReached?.Invoke();
    }
  }
}
