
using Godot;
using System;

public class Guard : BaseEnemy
{
    private FSM.State STATE_IDLE, STATE_WALKING, STATE_SHOOTING;
    private Position2D cannon;

    private RigidBody2D[] rigidParts;

    public override void _Ready()
    {
        cannon = GetNode<Position2D>("Body/Cannon");

        rigidParts = new RigidBody2D[4];
        rigidParts[0] = GetNode<RigidBody2D>("Body/Parts/BodyRB");
        rigidParts[1] = GetNode<RigidBody2D>("Body/Parts/BodyRB/02 body/03 L-leg-1/04 L-leg-2/LeftPikeRB");
        rigidParts[2] = GetNode<RigidBody2D>("Body/Parts/BodyRB/02 body/06 R-leg-1/RightRB");
        rigidParts[3] = GetNode<RigidBody2D>("Body/Parts/BodyRB/02 body/CannonRB");

        base._Ready();

        hp = 10;
    }

    // public void Explode(Bullet bullet)
    // {
    //     StopAnimation();
    //     motionCollisionShape.Disabled = true;
    //     Random random = new Random();
    //     foreach (RigidBody2D part in rigidParts)
    //     {
    //         part.Mode = RigidBody2D.ModeEnum.Rigid;
    //         part.Sleeping = false;
    //         part.GravityScale = 7;

    //         float r = bullet.Direction.x;//1 - 2 * (float)random.NextDouble();
    //         float fx = 250 * r;
    //         part.Inertia = 600;
    //         part.ApplyImpulse(new Vector2(20, 20), new Vector2(fx, -400));
    //     }
    //     PlayAnimation(ANIMATION_DISSAPEAR);
    // }

    protected override void SetupMovement()
    {
        Movement.Horizontal = 1;
    }

    protected override FSM BuildStateMachine()
    {
        STATE_IDLE = new FSM.State(OnIdleUpdate, OnIdleEnter);
        STATE_WALKING = new FSM.State(OnWalkingUpdate, OnWalkingEnter);
        STATE_SHOOTING = new FSM.State(OnShootingUpdate, OnShootEnter);

        return new FSM(STATE_WALKING);
    }

    public FSM.State OnIdleUpdate(float dt)
    {
        Move(dt);
        return STATE_IDLE;
    }

    public void OnIdleEnter()
    {
        PlayAnimation(ANIMATION_IDLE);
    }

    public FSM.State OnWalkingUpdate(float dt)
    {
        Movement.ApplyMotion(Movement.Horizontal);
        Move(dt);
        return STATE_IDLE;
    }

    public void OnWalkingEnter()
    {

    }

    public FSM.State OnShootingUpdate(float dt)
    {
        return STATE_IDLE;
    }

    public void OnShootEnter()
    {

    }

    // public override void OnDied()
    // {
    //     base.OnDied();
    //     // Explode();
    // }

}
