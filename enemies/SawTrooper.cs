
using Godot;
using System;

public class SawTrooper : BaseEnemy
{
    private RayCast2D floorRay;
    protected RayCast2D frontRay;

    public FSM.State STATE_MOVING { get; protected set; }
    public WaitState STATE_TURNING { get; protected set; }
    public float TurnTime = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        SetInitialState();
        floorRay = GetNode<RayCast2D>("Body/FloorRay");
        frontRay = GetNode<RayCast2D>("Body/FrontRay");
        hp = MoonHunter.Constants.SAW_TROOPER_ENERGY;
    }

    protected void SetInitialState()
    {
        hp = MoonHunter.Constants.SAW_TROOPER_ENERGY;
        TouchDamage = MoonHunter.Constants.TOUCH_DAMAGE;
        machine.ForceState(STATE_MOVING);
        Movement.InputHorizonalSpeed = 2 * MoonHunter.Constants.TILE_SIZE;
        Movement.Horizontal = -1;
    }

    public bool HasObstacleOrVoid()
    {
        return frontRay.IsColliding() || !floorRay.IsColliding();
    }

    protected override FSM BuildStateMachine()
    {
        STATE_MOVING = new FSM.State(OnMovingUpdate, OnMovingEnter) { Name = "Moving" };
        STATE_TURNING = new WaitState(TurnTime, STATE_MOVING) { Name = "Turning", OnExit = OnTurningExit };
        return new FSM(STATE_MOVING);
    }

    protected void OnMovingEnter()
    {
        statusAnimationPlayer.PlaybackSpeed = 2.0f;
        statusAnimationPlayer.Play("Rotate");
    }

    protected FSM.State OnMovingUpdate(float delta)
    {
        if (Movement.IsGrounded && HasObstacleOrVoid())
            return STATE_TURNING;

        Movement.ApplyMotion(Movement.Horizontal);
        Move(delta);

        return STATE_MOVING;
    }

    protected void OnTurningExit()
    {
        Movement.Horizontal *= -1;
        Movement.ApplyMotion(Movement.Horizontal);
        Move(0.001f);
    }
}
