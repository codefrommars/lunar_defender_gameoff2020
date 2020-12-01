
using Godot;
using System;

public class Weapon : Node2D
{
    public FSM.State STATE_READY { get; protected set; }
    public WaitState STATE_TIMEOUT { get; protected set; }

    public const string ANIMATION_RECOIL = "Recoil";
    [Export]
    private PackedScene projectileType;

    [Export]
    public float TimeoutDuration { get; set; } = 0.125f;

    private AudioStreamPlayer2D shootSound;
    private Position2D cannon;
    private AnimationPlayer animationPlayer;
    private CPUParticles2D bulletCaseParticle;

    protected FSM machine;
    private Vector2 aimDirection;
    public Hunter Hunter { get; set; }

    public bool Enabled { get; set; } = true;


    public static Weapon Instantiate()
    {
        PackedScene w = GD.Load<PackedScene>("res://objects/Weapon.tscn");
        return (Weapon)w.Instance();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cannon = GetNode<Position2D>("Cannon");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        bulletCaseParticle = GetNode<CPUParticles2D>("BulletCaseParticle");
        shootSound = GetNode<AudioStreamPlayer2D>("ShootAudioPlayer");


        STATE_READY = new FSM.State(WeaponReadyUpdate) { Name = "Ready" };
        STATE_TIMEOUT = new WaitState(TimeoutDuration, STATE_READY) { Name = "Reloading" };
        machine = new FSM(STATE_READY);
    }

    public void Shoot()
    {
        Bullet bullet = MoonHunter.Instance.NewProjectile<Bullet>(projectileType);
        aimDirection = Mathf.Polar2Cartesian(1.0f, GlobalRotation);
        Vector2 normal = new Vector2(aimDirection.y, -aimDirection.x);
        Random rand = new Random();
        float randAmplitude = 15;
        float randY = (float)(randAmplitude * (1 - 2 * rand.NextDouble()));
        bullet.Position = cannon.GlobalPosition + normal * randY;


        bullet.SetBulletRotation(GlobalRotation);
        // bullet.Rotation = GlobalRotation;
        // shootSound.Play();
        SoundManager.Instance.ShotAudioPlayer.Play();

        MoonHunterState.Shots += 1;
        // Hunter.Movement.Recoil(-aimDirection, 10 * MoonHunter.Constants.TILE_SIZE);

        //PlayAnimation(ANIMATION_RECOIL);
        // owner.Movement.ApplyForce(-bullet.Direction * 0.5f * 16 * 16);
        //owner.Movement.ApplyForce(new Vector2(-owner.Movement.FacingDirection, -1) * 2f * 16 * 16);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!Enabled)
            return;

        PollDirection();
        machine.Update(delta);
    }

    public void ShootLaserBeam()
    {
        LaserBeam laserBeam = MoonHunter.Instance.AddLaserBeam(cannon.GlobalPosition, cannon.GlobalRotation);
        laserBeam.Shoot();
        // cannon.AddChild(laserBeam);
    }

    protected void PollDirection()
    {
        Vector2 direction = InputManager.Direction;
        direction.x = Math.Abs(direction.x);
        Rotation = direction.Angle();
    }

    public void PlayAnimation(string anim)
    {
        animationPlayer.Play(anim);
    }

    #region WeaponReady

    public FSM.State WeaponReadyUpdate(float delta)
    {
        if (InputManager.ShootPressed)
        {
            Shoot();
            return STATE_TIMEOUT;//.Reset(TimeoutDuration);
        }

        return STATE_READY;
    }

    #endregion
}
