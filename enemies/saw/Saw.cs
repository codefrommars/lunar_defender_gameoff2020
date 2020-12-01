
using Godot;
using System;

public class Saw : BaseEnemy
{
    protected FSM.State STATE_WALKING;
    protected RayCast2D frontRay;

    public override void _Ready()
    {
        base._Ready();
        SetInitialState();
        frontRay = GetNode<RayCast2D>("Body/FrontRay");
        hp = MoonHunter.Constants.SAW_ENERGY;
    }

    protected void SetInitialState()
    {
        hp = MoonHunter.Constants.SAW_ENERGY;
        TouchDamage = 10;
        machine.ForceState(STATE_WALKING);
    }

    protected override void SetupMovement()
    {
        base.SetupMovement();
        Movement.InputHorizonalSpeed = 3 * MoonHunter.Constants.TILE_SIZE;
        Movement.Horizontal = 1;
    }

    protected override FSM BuildStateMachine()
    {
        // Movement.GravityScale = 0;
        STATE_WALKING = new FSM.State(OnWalkingUpdate);
        return new FSM(STATE_WALKING);
    }

    public FSM.State OnWalkingUpdate(float delta)
    {
        if (frontRay.IsColliding())
            Movement.Horizontal *= -1;

        Movement.ApplyMotion(Movement.Horizontal);
        Move(delta);

        return STATE_WALKING;
    }

    public void OnWalkingEnter()
    {
        statusAnimationPlayer.PlaybackSpeed = 3.0f;
        statusAnimationPlayer.Play("Rotate");
    }

    public void OnWalkingExit()
    {

    }
}
