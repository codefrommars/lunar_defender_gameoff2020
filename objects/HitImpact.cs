using Godot;
using System;

public class HitImpact : CPUParticles2D
{
    private Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = Lifetime;
    }

    public void Start()
    {
        timer.Start();
        Emitting = true;
    }

    public void OnTimeOut()
    {
        QueueFree();
    }
}
