using Godot;
using System;

public class Dust : CPUParticles2D
{
    private Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = Lifetime;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public void Start()
    {
        timer.Start();
        Emitting = true;
    }
    private void OnTimeOut()
    {
        // Replace with function body.
        QueueFree();
    }
}



