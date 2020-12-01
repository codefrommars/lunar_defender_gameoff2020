using Godot;
using System;

public class Explosion : CPUParticles2D
{
    private Timer timer;
    private AnimatedSprite sprite;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = Lifetime;

        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public void Start()
    {
        timer.Start();
        Emitting = true;
        sprite.Play();
    }

    public void OnTimeOut()
    {
        Emitting = false;
    }

    public void OnAnimationFinished()
    {
        QueueFree();

    }
}
