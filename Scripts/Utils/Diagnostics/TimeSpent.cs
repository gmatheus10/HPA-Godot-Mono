using Godot;
using System.Diagnostics;
public class TimeSpent
{
    Stopwatch watch;
    public TimeSpent()
    {
        watch = Stopwatch.StartNew();
    }
    public void Measure(string message)
    {
        watch.Stop();
        GD.Print($"{message}: {watch.ElapsedMilliseconds } milliseconds");
        
    }

}