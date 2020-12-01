using Godot;
using System;

public class LaserBeam : Area2D
{
    [Export]
    public float MaxWidth { get; set; } = 60;
    public int Damage { get; set; } = MoonHunter.Constants.LASER_BEAM_DAMAGE;

    private Line2D body;
    private Tween lifeTween;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        body = GetNode<Line2D>("Body");
        // CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        lifeTween = new Tween();
        AddChild(lifeTween);
    }

    public void Shoot(float duration = 0.5f)
    {
        ShowBody();
        SoundManager.Instance.LaserBeamAudioPlayer.Play();
        SceneTreeTimer timer = GetTree().CreateTimer(duration);
        timer.Connect("timeout", this, nameof(HideBody));
    }

    private void ShowBody()
    {
        float duration = 0.2f;
        lifeTween.StopAll();
        lifeTween.InterpolateProperty(body, "width", 0f, MaxWidth, duration);
        lifeTween.InterpolateCallback(this, duration, nameof(DoDamage));
        lifeTween.Start();
    }

    private void DoDamage()
    {
        Godot.Collections.Array array = GetOverlappingBodies();
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] is HittablePart)
            {
                HittablePart part = (HittablePart)array[i];
                part.DoDamage(Damage);
            }

            if (array[i] is Box)
            {
                Box box = array[i] as Box;
                box.DoDamage(Damage);
            }
        }
    }

    private void HideBody()
    {

        float duration = 0.1f;
        lifeTween.StopAll();
        lifeTween.InterpolateProperty(body, "width", MaxWidth, 0f, duration);
        lifeTween.InterpolateCallback(this, duration, nameof(RemoveSelf));
        lifeTween.Start();
    }

    private void RemoveSelf()
    {
        QueueFree();
    }
}
