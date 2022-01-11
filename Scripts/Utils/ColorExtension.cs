using Godot;
public static class ColorExtension
{

 public static Color red(this Color c)
 {
  return new Color("#ff0000");
 }
 public static Color blue(this Color c)
 {
  return new Color("#0000ff");
 }
 public static Color green(this Color c)
 {
  return new Color("#008000");
 }
 public static Color black(this Color c)
 {
  return new Color("#000000");
 }
 public static Color white(this Color c)
 {
  return new Color("#ffffff");
 }
 public static Color yellow(this Color c)
 {
  return new Color("#ffff00");
 }
}