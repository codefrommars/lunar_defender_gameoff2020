
using Godot;
using System;

public class Walker : BaseEnemy
{
    private FSM.State STATE_WALKING;

    // private AnimatedSprite animatedSprite;
    private RayCast2D frontRay;

    public override void _Ready()
    {
        // animatedSprite = GetNode<AnimatedSprite>("Body/AnimatedSprite");
        frontRay = GetNode<RayCast2D>("Body/FrontRay");

        base._Ready();

        hp = MoonHunter.Constants.WALKER_ENERGY;
    }

    protected override FSM BuildStateMachine()
    {
        STATE_WALKING = new FSM.State(OnWalkingUpdate, OnWalkingEnter, OnWalkingExit);
        return new FSM(STATE_WALKING);
    }

    protected override void SetupMovement()
    {
        base.SetupMovement();
        Movement.InputHorizonalSpeed = 1.5f * MoonHunter.Constants.TILE_SIZE;
        Movement.Horizontal = 1;
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
        // animatedSprite.Play("Walking");
        statusAnimationPlayer.Stop();
        statusAnimationPlayer.Play("Walking");
    }

    public void OnWalkingExit()
    {
        // animatedSprite.Stop();
    }




}
