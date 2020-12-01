using Godot;

public class BulletExplosion : CPUParticles2D
{
    private Timer timer;
    
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
