using Godot;
using System;

public class Mine : KinematicBody2D
{
    const float gravity = 1000.0f;
    private Vector2 velocity;
    private AnimatedSprite sprite;
    private Area2D damageArea;


    // public static int Placed = 0;

    public int Damage { get; set; } = 20;

    public override void _Ready()
    {
        damageArea = GetNode<Area2D>("DamageArea");
        sprite = GetNode<AnimatedSprite>("Sprite");
        // Placed++;
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity.y += delta * gravity;
        velocity = MoveAndSlideWithSnap(velocity, new Vector2(60, 60), Vector2.Up, true, 2, 0.785398f, false);
        // var collision = MoveAndCollide(velocity * delta, false);

        // if (collision != null)
        // {
        //     velocity.y = 0;
        // }

    }

    public void OnActivationAreaBodyEntered(Node body)
    {
        if (body is BaseEnemy)
        {
            Arm();
        }
    }

    public void Arm()
    {
        sprite.Modulate = new Color(1, 0, 0, 1);
        SceneTreeTimer timer = GetTree().CreateTimer(0.15f);
        timer.Connect("timeout", this, nameof(Explode));
        //Beep ?
    }

    private void Explode()
    {
        MoonHunter.Instance.AddExplosion(GlobalPosition);
        SoundManager.Instance.Explosion5AudioPlayer.Play();
        Godot.Collections.Array array = damageArea.GetOverlappingBodies();
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] is HittablePart)
            {
                HittablePart part = (HittablePart)array[i];
                part.DoDamage(Damage);
            }
        }
        // GD.Print("Explode: " + array.Count);
        // Placed--;
        MoonHunter.Instance.MineExploded();
        QueueFree();
    }
}
