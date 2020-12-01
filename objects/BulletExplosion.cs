using Godot;

public class BulletExplosion : CPUParticles2D
{
    private Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = Lifetime;
    }

    public void Start(float rotation)
    {
        Rotation = rotation;
        timer.Start();
        Emitting = true;
    }

    public void OnTimeOut()
    {
        QueueFree();
    }
}
