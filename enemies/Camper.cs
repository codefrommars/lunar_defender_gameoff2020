
using Godot;
using System;

public class Camper : BaseEnemy
{
    protected RayCast2D frontFloorRay;
    protected RayCast2D backFloorRay;
    protected RayCast2D frontRay;
    protected RayCast2D backRay;
    protected Position2D cannonPosition;

    private AnimationPlayer wheelsPlayer, cannonPlayer;
    private Node2D cannonHolder;

    public FSM.State STATE_MOVING { get; protected set; }
    public TimedState STATE_AIMING { get; protected set; }
    public FSM.State STATE_SHOOTING { get; protected set; }

    private float TurnTime = 0.1f;
    private float movingTimeLeft = 2.0f;
    private float speedInTiles = 1.0f;
    private int lastOrientation;

    protected bool MovingFront { get { return Movement.Horizontal == 1; } }
    protected bool MovingBack { get { return Movement.Horizontal == -1; } }

    public override void _Ready()
    {
        cannonPosition = GetNode<Position2D>("Body/body/CannonHolder/cannon/CannonPosition");
        wheelsPlayer = GetNode<AnimationPlayer>("WheelsAnimationPlayer");
        cannonPlayer = GetNode<AnimationPlayer>("CannonAnimationPlayer");
        cannonHolder = GetNode<Node2D>("Body/body/CannonHolder");
        frontFloorRay = GetNode<RayCast2D>("Body/FrontFloorRay");
        backFloorRay = GetNode<RayCast2D>("Body/BackFloorRay");
        frontRay = GetNode<RayCast2D>("Body/FrontRay");
        backRay = GetNode<RayCast2D>("Body/BackRay");

        base._Ready();

        SetInitialState();
    }

    protected void SetInitialState()
    {
        hp = 450;
        TouchDamage = 50;
        machine.ForceState(STATE_MOVING);
        Movement.InputHorizonalSpeed = speedInTiles * MoonHunter.Constants.TILE_SIZE;
        Movement.Horizontal = 1;
    }

    public override void Start()
    {
        base.Start();
        statusAnimationPlayer.Play("On");
    }

    protected override FSM BuildStateMachine()
    {
        STATE_MOVING = new FSM.State(OnMovingUpdate) { Name = "Moving", OnExit = StopCar };
        STATE_AIMING = new TimedState(3.0f, OnAimUpdate)
        {
            Name = "Aiming",
            OnEnter = StartAiming
        };
        STATE_SHOOTING = new WaitState(2, STATE_MOVING) { Name = "Shooting", OnEnter = Shoot, OnExit = SelectRandomDirection };

        return new FSM(STATE_NO_ACTION);
    }

    protected void SelectRandomDirection()
    {
        movingTimeLeft = MoonHunter.Instance.GetRandom(1.0f, 3.0f);
        SetMovementOrientation(MoonHunter.Instance.GetRandomSign());
    }

    private void SetMovementOrientation(int value)
    {
        float speed = speedInTiles * MoonHunter.Constants.TILE_SIZE;
        if (value != 0)
        {
            lastOrientation = value;
        }
        else
        {
            speed = 0;
        }

        Movement.InputHorizonalSpeed = speed;
        Movement.Horizontal = value;

        if (Movement.Horizontal == 0)
        {
            Movement.InputHorizonalSpeed = 0;
            wheelsPlayer.Stop(false);
        }
        else if (Movement.Horizontal > 0)
        {
            wheelsPlayer.Play("Rotate");
        }
        else
        {
            wheelsPlayer.PlayBackwards("Rotate");
        }

    }

    protected void OnMovingExit()
    {

    }

    protected bool CanMove()
    {
        if (MovingBack)
            return backFloorRay.IsColliding() && !backRay.IsColliding();

        return frontFloorRay.IsColliding() && !frontRay.IsColliding();
    }


    protected FSM.State OnMovingUpdate(float delta)
    {
        if (Movement.IsGrounded && !CanMove())
            Turn();

        movingTimeLeft -= delta;
        if (movingTimeLeft < 0)
            return STATE_SHOOTING;

        Movement.ApplyMotion(Movement.Horizontal);
        Move(delta);

        return STATE_MOVING;
    }

    protected void StartAiming()
    {
        STATE_AIMING.ResetTime();
    }

    protected FSM.State OnAimUpdate(float delta)
    {
        if (STATE_AIMING.IsOver)
            return STATE_SHOOTING;

        AimToPlayer();

        return STATE_AIMING;
    }

    protected void AimToPlayer()
    {
        Vector2 pp = MoonHunter.Instance.Player.GlobalPosition;
        Vector2 ch = cannonHolder.GlobalPosition;
        cannonHolder.Rotation = pp.AngleToPoint(ch);
    }

    protected void Turn()
    {
        SetMovementOrientation(-1 * lastOrientation);
    }

    protected void StopCar()
    {
        SetMovementOrientation(0);
        Movement.ApplyMotion(0);
    }

    public override void FaceDirection()
    {

    }

    public override void _Process(float delta)
    {
        debugLabel.Text = machine.CurrentState.Name + ": " + Movement.Horizontal;
        if (Enabled)
            AimToPlayer();
    }

    public virtual void Shoot()
    {
        Bullet bullet = ShootAt(BulletType, cannonPosition.GlobalPosition, cannonHolder.Rotation);
        ConfigureBullet(bullet);
    }
}
