using System;
using Godot;
public class JunctionArgs : EventArgs
{
    public Junction StartJunction { get; private set; }
    public Junction EndJunction { get; private set; }

    public JunctionArgs(Junction startJunction, Junction endJunction)
    {
        this.StartJunction = startJunction;
        this.EndJunction = endJunction;
    }
}