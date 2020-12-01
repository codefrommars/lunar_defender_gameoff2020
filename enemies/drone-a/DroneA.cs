
using Godot;
using System;

public class DroneA : BaseEnemy
{
    private WaitState STATE_MOVING_TO;
    private WaitState STATE_SHOOT_ONE;
    private WaitState STATE_SHOOT_THREE;

    private Tween moveTween;
    private float floatingTime;

    private float movingTime = 1.0f;
    private float shootingTime = 1.5f;

    public Rect2 FlyingArea { get; set; }

    public override void _Ready()
    {
        moveTween = new Tween();
        AddChild(moveTween);

        base._Ready();

        hp = 150;
        floatingTime = 0;
    }

    //Move to place
    //Shoot player
    protected override FSM BuildStateMachine()
    {
        //STATE_MOVING_TO = new WaitState(OnWalkingUpdate, OnWalkingEnter, OnWalkingExit);

        STATE_SHOOT_ONE = new WaitState(shootingTime)
        {
            OnExit = Shoot,
            //OnExit = OnShootExit,
        };

        // STATE_SHOOT_THREE = new FSM.State(OnShootingThreeUpdate, OnShootThreeEnter, OnShootExit);

        STATE_MOVING_TO = new WaitState(movingTime, STATE_SHOOT_ONE)
        {
            OnEnter = OnMovingToEnter,
            OnExit = OnMovingToExit
            // OnReenterCallback = OnMovingToExit
        };

        STATE_SHOOT_ONE.NextState = STATE_MOVING_TO;

        return new FSM(STATE_NO_ACTION);
    }

    public void MoveTo(Vector2 percentage, float duration)
    {
        Vector2 target = MoonHunter.Instance.RelativeToGlobal(percentage);
        // GD.Print("Move from: " + GlobalPosition + " to: " + target);
        moveTween.InterpolateProperty(this, "position", GlobalPosition, target, duration);
        moveTween.Start();
    }

    // protected override void SetupMovement()
    // {
    //     base.SetupMovement();
    // }

    public override void ProcessMovement(float delta)
    {
        AddFloating(delta);
    }

    private void AddFloating(float delta)
    {
        floatingTime += delta;
        float y = (float)(20 * Math.Sin(floatingTime * 2 * Math.PI));
        body.Position = new Vector2(0, y);

    }

    protected override void ConfigureBullet(Bullet bullet)
    {
        bullet.Damage = MoonHunter.Constants.DEFAULT_BULLET_ENEMY_DAMAGE;
        bullet.Speed = 800;
    }

    public void OnMovingToEnter()
    {
        //Pick a random position
        //0.2f, 0.8f, 0.2f, 0.5f
        Vector2 position = MoonHunter.Instance.GetRandom(FlyingArea);
        // float duration = 2.0f;

        MoveTo(position, movingTime);

    }

    public void OnMovingToExit()
    {
    }

    public virtual void Shoot()
    {
        Bullet bullet = ShootAt(BulletType, GlobalPosition, MoonHunter.Instance.Player.GlobalPosition);
        ConfigureBullet(bullet);
    }

    // public void OnShootEnter()
    // {
    //     Shoot()
    // }

    public void OnShootThreeEnter()
    {
        Bullet bullet = ShootAt(BulletType, GlobalPosition, MoonHunter.Instance.Player.GlobalPosition);
        ConfigureBullet(bullet);

        // Vector2 direction = GlobalPosition.DirectionTo(MoonHunter.Instance.Player.GlobalPosition);

        // Bullet bullet = MoonHunter.Instance.NewProjectile<Bullet>(BulletType);
        // bullet.Position = GlobalPosition;
        // bullet.SetBulletRotation(direction.Angle());
    }

    // public void OnShootExit()
    // {
    //     // animatedSprite.Stop();
    // }

    private float inShootingTime;

    public override void SetParameters(params object[] parameters)
    {
        if (parameters.Length == 0)
            return;

        FlyingArea = (Rect2)parameters[0];
    }

    public override void Start()
    {
        machine.ForceState(STATE_MOVING_TO);
    }
}
