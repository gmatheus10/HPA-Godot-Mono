using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
public static class Utils
{
 private readonly static int MOVE_DIAGONAL_COST = 14;
 private readonly static int MOVE_STRAIGHT_COST = 10;
 // public static Vector2 GetMousePosition()
 // {
 //  return Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector2(0, 0, 10);
 // }

 public static float JunctionDistance(Junction a, Junction b)
 {
  return (a.WorldPosition.DistanceTo(b.WorldPosition));
 }
 public static int ManhatamDistance(Vector2 GridPositionA, Vector2 GridPositionB)
 {
  Vector2 a = GridPositionA;
  Vector2 b = GridPositionB;
  int xDistance = (int)Mathf.Abs(a.x - b.x);
  int yDistance = (int)Mathf.Abs(a.y - b.y);
  int remaining = Mathf.Abs(xDistance - yDistance);
  return Mathf.RoundToInt(MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining);
 }
 public static int ManhatamDistance(Cell a, Cell b)
 {
  return ManhatamDistance(a.GridPosition, b.GridPosition);
 }
 public static int ManhatamDistance(Cluster a, Cluster b)
 {
  float xDistance = Mathf.Abs(a.OriginPosition.x - b.OriginPosition.x);
  float yDistance = Mathf.Abs(a.OriginPosition.y - b.OriginPosition.y);
  return Mathf.RoundToInt(xDistance + yDistance);
 }

 public static int factorial(int n)
 {
  if (n == 0 || n == 1)
  {
   return 1;
  }
  int fatorial = n;
  for (int i = n - 1; i >= 1; i--)
  {
   fatorial *= i;
  }
  return fatorial;
 }
}
