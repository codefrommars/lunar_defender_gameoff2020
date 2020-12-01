using Godot;
using System;

public class Grenade : RigidBody2D
{
    private float armedDuration = 2.0f;
    private float imminentDuration = 1.0f;

    private AnimationPlayer animationPlayer;
    private Area2D damageArea;


    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        damageArea = GetNode<Area2D>("DamageArea");
        Arm();
    }

    public void Arm()
    {
        SceneTreeTimer timer = GetTree().CreateTimer(armedDuration);
        timer.Connect("timeout", this, nameof(Imminent));
        animationPlayer.Play("Arm");
    }

    private void Imminent()
    {
        SceneTreeTimer timer = GetTree().CreateTimer(imminentDuration);
        timer.Connect("timeout", this, nameof(Explode), null, (uint)ConnectFlags.Deferred);
        animationPlayer.Play("Imminent");
    }

    private void Explode()
    {
        MoonHunter.Instance.AddExplosion(GlobalPosition);

        Godot.Collections.Array array = damageArea.GetOverlappingBodies();
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] is HittablePart)
            {
                HittablePart part = (HittablePart)array[i];
                part.DoDamage(20);
            }
        }
        // GD.Print("Explode: " + array.Count);

        QueueFree();
    }
    // private MoonMovement movement;

    // // Called when the node enters the scene tree for the first time.
    // public override void _Ready()
    // {
    //     movement = new MoonMovement();
    //     //Setup Movement
    //     movement.Horizontal = 0;
    //     movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HORIZONTAL_SPEED;
    //     movement.SetupJumpPhysics(MoonHunter.Constants.PLAYER_JUMP_HEIGHT, MoonHunter.Constants.PLAYER_MIN_JUMP_HEIGHT, MoonHunter.Constants.PLAYER_JUMP_DURATION);
    //     movement.FloorDamp = 0.9f;
    //     movement.AirDamp = 1.0f;
    //     movement.UseHorizontalDamp = true;
    // }

    // public override void _PhysicsProcess(float delta)
    // {
    //     movement.Move(this, delta);
    // }


}
