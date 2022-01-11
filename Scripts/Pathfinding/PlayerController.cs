using Godot;
using System;

public class PlayerController : PositionSender
{
 //this script sends the player position and mouse position, when clicked, as eventArg to whoever gets
 //subscribed: Hierarchical_Pathfinding.cs
 private Mouse mouse;
 private CreateGrid createGrid;
 private Grid<Cell> grid;
 /////////////////////////////////////////////////////////////////////////////////////////

 public override void _Ready()
 {
  createGrid = GetNode<CreateGrid>("../../CreateGrid");
  mouse = GetNode<Mouse>("../../Mouse");
  grid = createGrid.Grid;
  mouse.MousePositionLeftButton += MouseLeftPosition;
 }
 public void MouseLeftPosition(Vector2 mousePosition)
 {
  SetJunctions(mousePosition);
  Update();
 }


 private void SetJunctions(Vector2 MousePosition)
 {
  Node2D player = GetParent<Node2D>();

  Junction startJunction = PositionToJunction(player.Position);
  Junction endJunction = PositionToJunction(MousePosition);
  JunctionArgs pos = new JunctionArgs(startJunction, endJunction);
  SendPositions(pos);

 }

 private Junction PositionToJunction(Vector2 position)
 {
  if (grid != null)
  {
   if (grid.IsInsideGrid(position))
   {
    Cell cell = grid.GetGridObject(position);
    Junction n = NewJunction(cell.WorldPosition);
    n.SetGridPosition(cell.GridPosition);
    return n;
   }
   return null;
  }
  return null;
 }
 private Junction NewJunction(Vector2 position)
 {
  Junction n = new Junction();
  n.SetPosition(position);
  return n;
 }
}
