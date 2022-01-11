using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
[Serializable]
public class Entrance
{
 public Cluster Cluster1 { get; private set; }
 public Cluster Cluster2;

 public Vector2 OriginPosition { get; private set; }
 public Vector2 EndPosition { get; private set; }

 public List<Cell> entranceTiles = new List<Cell>();
 public List<Cell> symmEntranceTiles = new List<Cell>();

 public List<Junction> entranceJunctions = new List<Junction>();

 public bool isBlocked = false;
 public Entrance()
 {

 }
 public void SetOriginPosition(Vector2 position)
 {
  this.OriginPosition = position;

 }

 public void SetClusters(Cluster Cluster1, Cluster Cluster2)
 {
  this.Cluster1 = Cluster1;
  this.Cluster2 = Cluster2;
 }
 public void SetClusters(Cluster[] pair)
 {
  if (pair.Length > 2)
  {
   throw new System.Exception("Cannot have more than 2 elements in argument array");
  }
  else
  {
   this.Cluster1 = pair[0];
   this.Cluster2 = pair[1];
  }
 }
 public void FillEntrance(List<Cell> entranceTiles)
 {
  this.entranceTiles = entranceTiles;
  this.OriginPosition = entranceTiles[0].WorldPosition;
  this.EndPosition = entranceTiles[entranceTiles.Count - 1].WorldPosition;
 }
 public void FillSymmEntrance(List<Cell> symmTiles)
 {
  symmEntranceTiles = symmTiles;

 }
 public Cluster[] GetClusters()
 {
  Cluster[] clusters = new Cluster[2];
  clusters[0] = Cluster1;
  clusters[1] = Cluster2;
  return clusters;
 }
 public List<Cell> GetEntrance()
 {
  return entranceTiles;
 }
 public List<Cell> GetSymm()
 {
  return symmEntranceTiles;
 }
 public void AddJunction(Junction Junction)
 {
  this.entranceJunctions.Add(Junction);
 }
 public Cell GetSymmetricalCell(Cell Reference)
 {
  int index = entranceTiles.IndexOf(Reference);
  try
  {
   Cell symmCell = symmEntranceTiles[index];
   return symmCell;
  }
  catch (System.Exception)
  {

   return null;
  }
 }
 public bool HaveEntrance(Cluster cluster1, Cluster cluster2)
 {

  bool entranceTilesInside = cluster1.IsEntranceInside(this.entranceTiles);
  bool symmTilesInside = cluster2.IsEntranceInside(this.symmEntranceTiles);
  if (entranceTilesInside && symmTilesInside)
  {
   return true;
  }
  return false;
 }
 public bool IsJunctionInside(Junction Junction)
 {
  return entranceJunctions.Contains(Junction);
 }


 public Entrance MergeEntrances(params Entrance[] entrances)
 {
  Entrance merged = new Entrance();


  try
  {
   merged.SetClusters(entrances[0].GetClusters());
   merged.OriginPosition = entrances[0].OriginPosition;
  }
  catch (System.Exception)
  {

   throw;
  }

  merged.EndPosition = entrances[entrances.Length - 1].EndPosition;

  List<Cell> mergedTiles = new List<Cell>();
  List<Cell> mergedSymmTiles = new List<Cell>();

  List<Junction> mergedJunctions = new List<Junction>();

  foreach (Entrance e in entrances)
  {
   mergedTiles = mergedTiles.Concat(e.entranceTiles).ToList();
   mergedSymmTiles = mergedSymmTiles.Concat(e.symmEntranceTiles).ToList();
   mergedJunctions = mergedJunctions.Concat(e.entranceJunctions).ToList();
  }
  merged.entranceTiles = mergedTiles;
  merged.symmEntranceTiles = mergedSymmTiles;
  merged.entranceJunctions = mergedJunctions;
  if (mergedJunctions.Count == 0)
  {
   merged.isBlocked = true;
  }
  return merged;
 }
}