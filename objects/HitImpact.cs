using Godot;
using System;

public class HitImpact : CPUParticles2D
{
    private Timer timer;

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
