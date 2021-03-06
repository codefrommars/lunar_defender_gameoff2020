

using Godot;

public class Character : KinematicBody2D
{
    public const string ANIMATION_NORMAL = "Normal";

    //Animations
    public const string ANIMATION_IDLE = "Idle";
    public const string ANIMATION_DISSAPEAR = "Dissapear";

    //For Debugging
    public static string[] STATE_NAME = new string[] { "Idle", "Run", "Jump", "Fall" };

    //Movement Component
    public MoonMovement Movement { get; } = new MoonMovement();
    public float SurfaceFriction { get; set; }
    public float EnvironmentFriction { get; set; }

    //Visual
    private Material defaultMaterial;
    //Node references
    protected Node2D body;
    // protected Node2D floorRaycasts;
    protected AnimationPlayer statusAnimationPlayer;
    protected CollisionShape2D motionCollisionShape;

    public Label debugLabel;

    public virtual bool Enabled { get; set; } = true;
    public bool Vulnerable { get; set; } = true;
    public bool Alive { get; private set; } = true;

    public static readonly FSM.State STATE_NO_ACTION = new FSM.State() { Name = "No_op" };

    //Health
    protected int hp = 200;

    //State machine
    protected FSM machine;

    public override void _Ready()
    {
        body = GetNode<Node2D>("Body");
        defaultMaterial = body.Material;
        debugLabel = GetNode<Label>("DebugLabel");
        motionCollisionShape = GetNode<CollisionShape2D>("MotionCollisionShape");
        statusAnimationPlayer = GetNode<AnimationPlayer>("StatusAnimationPlayer");
        SetupMovement();
        machine = BuildStateMachine();
    }

    protected virtual FSM BuildStateMachine()
    {
        return new FSM(STATE_NO_ACTION);
    }

    protected virtual void SetupMovement()
    {
        EnvironmentFriction = MoonHunter.Constants.AIR_FRICTION;
        SurfaceFriction = MoonHunter.Constants.FLOOR_FRICTION;

        Movement.MaxTotalSpeed = MoonHunter.Constants.PLAYER_MAX_TOTAL_SPEED;
        Movement.MaxGravitySpeed = MoonHunter.Constants.PLAYER_MAX_GRAVITY_SPEED;

        Movement.Horizontal = 0;
        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HORIZONTAL_SPEED;
        Movement.SetupJumpPhysics(MoonHunter.Constants.PLAYER_JUMP_HEIGHT, MoonHunter.Constants.PLAYER_MIN_JUMP_HEIGHT, MoonHunter.Constants.PLAYER_JUMP_DURATION);
    }

    public override void _PhysicsProcess(float delta)
    {

        if (!Enabled)
            return;

        machine.Update(delta);
        ProcessMovement(delta);
        FaceDirection();
    }

    public override void _Process(float delta)
    {
        debugLabel.Text = hp + "";
    }

    public virtual void ProcessMovement(float delta)
    {
        Move(delta);
    }

    public void Move(float delta)
    {
        Movement.Move(this, delta);
        Movement.IsGrounded = CheckIsGrounded();
    }

    protected bool CheckIsGrounded()
    {
        return IsOnFloor();
    }

    public void PlayAnimation(string animationName)
    {
        if (statusAnimationPlayer.CurrentAnimation == animationName)
            return;

        statusAnimationPlayer.Play(animationName);
    }

    public void StopAnimation()
    {
        statusAnimationPlayer.Stop();
    }

    public virtual void FaceDirection()
    {
        body.Scale = new Vector2(Movement.FacingDirection, 1);
    }

    public virtual bool OnHit(Bullet bullet)
    {
        DoDamage(bullet.Damage, bullet.Direction);
        return true;
    }

    public void DoDamage(int damage, Vector2 direction = new Vector2())
    {
        Blink();
        LoseHP(damage);
    }

    public void Blink(float duration = 0.05f)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(duration);
        timer.Connect("timeout", this, nameof(ResetMaterial));
        body.Material = MoonHunter.Instance.WHITE_MATERIAL;
    }

    private void ResetMaterial()
    {
        body.Material = defaultMaterial;
    }

    protected void LoseHP(int damage)
    {
        hp -= damage;

        if (hp <= 0)
            Die();
    }

    public virtual void Die()
    {
        Alive = false;
        Vulnerable = false;
    }

    public void OnDissapeared()
    {
        QueueFree();
    }

    public virtual void Start()
    {
    }
}
