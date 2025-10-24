using Godot;
using System;

public abstract partial class Upgrade : Node
{
    public virtual void OnPickup(){}
    public virtual void OnLose(){}
}
