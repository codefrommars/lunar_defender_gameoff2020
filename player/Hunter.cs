
using Godot;
using System;

public class Hunter : Character
{

    public float HitDuration = 0.5f;
    public float HitInvencibility = 2.0f;

    //Motion states
    public FSM.State STATE_IDLE { get; protected set; }
    public FSM.State STATE_RUN { get; protected set; }
    public FSM.State STATE_JUMP { get; protected set; }
    public FSM.State STATE_FALL { get; protected set; }
    public TimedState STATE_HURT { get; protected set; }
    public FSM.State STATE_DASH { get; protected set; }

    //private bool dashEnabled = true;
    private AnimationPlayer invincibleAnimPlayer;
    private AnimationPlayer bodyAnimationPlayer;
    private RayCast2D boxBuildRay;
    private Area2D boxBuildArea;

    private FSM invencibilityMachine;
    public WaitState STATE_HURT_INVINCIBLE { get; protected set; }
    public FSM.State STATE_VULNERABLE { get; protected set; }

    private Weapon weapon;
    private Position2D weaponSlot;
    private Position2D feetPosition;

    private Sprite head, hurtPose;
    private Node2D mesh;

    private Node2D boxProp;

    public override bool Enabled
    {
        set
        {
            base.Enabled = value;
            weapon.Enabled = value;
        }
    }

    public int Energy
    {
        get { return hp; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        weaponSlot = GetNode<Position2D>("Body/WeaponSlot");
        feetPosition = GetNode<Position2D>("Body/Feet");
        invincibleAnimPlayer = GetNode<AnimationPlayer>("InvincibleAnimationPlayer");
        bodyAnimationPlayer = GetNode<AnimationPlayer>("BodyAnimationPlayer");
        head = GetNode<Sprite>("Body/mesh/chest/HeadSlot/head");
        hurtPose = GetNode<Sprite>("Body/hurt");
        mesh = GetNode<Node2D>("Body/mesh");
        boxBuildRay = GetNode<RayCast2D>("BoxBuildRay");
        boxBuildArea = GetNode<Area2D>("Body/BoxBuildArea");
        boxProp = boxBuildArea.GetNode<Node2D>("BoxProp");

        base._Ready();
        AttachWeapon(Weapon.Instantiate());
        hp = MoonHunter.Constants.INITIAL_PLAYER_HP;
    }

    public static Hunter Instance()
    {
        return (Hunter)GD.Load<PackedScene>("res://player/Hunter.tscn").Instance();
    }

    protected override void SetupMovement()
    {
        base.SetupMovement();
        Movement.Horizontal = 1;
    }

    protected override FSM BuildStateMachine()
    {
        STATE_IDLE = new FSM.State(OnIdleUpdate, OnIdleStart) { Name = "Idle" };
        STATE_RUN = new FSM.State(OnRunUpdate, OnRunStart, OnRunEnd) { Name = "Run" };
        STATE_JUMP = new FSM.State(OnJumpUpdate, OnJumpStart, OnJumpEnded) { Name = "Jump" };
        STATE_FALL = new FSM.State(OnFallUpdate, OnFallStart, OnFallEnd) { Name = "Fall" };
        STATE_HURT = new TimedState(HitDuration, OnHurtUpdate)
        {
            Name = "Hurt",
            OnEnter = OnHurtEnter,
            OnExit = OnHurtExit
        };

        STATE_DASH = new FSM.State(OnDashUpdate, OnDashStart, OnDashEnded) { Name = "Dash" };


        //Invencibility
        STATE_VULNERABLE = new FSM.State();
        STATE_HURT_INVINCIBLE = new WaitState(HitInvencibility, STATE_VULNERABLE)
        {
            Name = "Hurt_Invincible",
            OnEnter = OnHurtInvincibleEnter,
            OnExit = OnHurtInvincibleExit
        };



        invencibilityMachine = new FSM(STATE_VULNERABLE);
        // STATE_HURT.OnEnter = OnHitEnter;
        // STATE_HURT.OnExit = OnHitExit;

        return new FSM(STATE_IDLE);
    }

    private bool slowDown = false;

    public override void _Process(float delta)
    {
        Vector2 direction = InputManager.Direction;
        invencibilityMachine.Update(delta);
        // debugLabel.Text = machine.CurrentState.Name;

        //Build ?

        if (Input.IsActionJustPressed("ui_1") && MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Blocks))
        {
            boxProp.Visible = true;
            // PromptBuildBox();
        }

        if (Input.IsActionJustReleased("ui_1") && MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Blocks))
        {
            boxProp.Visible = false;
            BuildBox();
        }

        if (Input.IsActionJustPressed("ui_2") && MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Mines))
        {
            AddMine();
        }

        if (Input.IsActionJustPressed("ui_3") && MoonHunter.Instance.GameState.IsUnlocked(MoonHunterState.Powerup.Beam))
        {
            ShootLaserBeam();

        }

        if (Input.IsActionJustPressed("ui_slow"))
        {
            GetTree().Quit();
        }

        // if (Input.IsActionJustPressed("ui_slow"))
        // {
        //     slowDown = !slowDown;
        //     if (slowDown)
        //         Engine.TimeScale = 0.1f;
        //     else
        //         Engine.TimeScale = 1;
        // }

        if (boxProp.Visible)
        {
            CalculateBoxPropColor();
        }
        //TODO Fix head orientation
        head.Rotation = weapon.Rotation * 0.5f;
        base._Process(delta);
    }

    private void CalculateBoxPropColor()
    {

        int overlapping = boxBuildArea.GetOverlappingBodies().Count;

        if (overlapping > 0)
            boxProp.Modulate = new Color(1, 0, 0, 0.5f);
        else
            boxProp.Modulate = new Color(1, 1, 1, 0.5f);

    }

    private bool CanAfford()
    {
        return (hp > MoonHunter.Constants.MIN_HP_FOR_PLACING);
    }

    private void BuildBox()
    {
        if (!CanAfford())
        {
            SoundManager.Instance.CancelAudioPlayer.Play();
            return;
        }

        // boxBuildRay.ForceRaycastUpdate();
        // Vector2 vec = boxBuildRay.GetCollisionPoint();
        int overlapping = boxBuildArea.GetOverlappingBodies().Count;
        if (overlapping > 0)
        { //Cant build
            SoundManager.Instance.CancelAudioPlayer.Play();
            return;
        }

        hp -= MoonHunter.Constants.BOX_COST;

        MoonHunter.Instance.AddBox(boxBuildArea.GlobalPosition);
    }

    private void ShootLaserBeam()
    {
        if (!CanAfford())
        {
            SoundManager.Instance.CancelAudioPlayer.Play();
            return;
        }

        hp -= MoonHunter.Constants.LASER_BEAM_COST;
        weapon.ShootLaserBeam();
    }

    private void ThrowGranade()
    {
        MoonHunter.Instance.AddGrenade(GlobalPosition, new Vector2(Movement.FacingDirection, -0.5f));
    }

    private void AddMine()
    {
        if (MoonHunter.Instance.CountPlacedMinesInStage() >= MoonHunter.Constants.MAX_MINES_IN_GAME)
        {
            SoundManager.Instance.CancelAudioPlayer.Play();
            return;
        }

        if (!CanAfford())
        {
            SoundManager.Instance.CancelAudioPlayer.Play();
            return;
        }

        hp -= MoonHunter.Constants.MINE_COST;


        MoonHunter.Instance.AddMine(GlobalPosition);
    }



    public void AttachWeapon(Weapon newWeapon)
    {
        weapon?.QueueFree();
        weapon = newWeapon;
        weapon.Hunter = this;
        weaponSlot.AddChild(newWeapon);
    }

    public Vector2 GetCollisionExtents()
    {
        RectangleShape2D shape = (RectangleShape2D)motionCollisionShape.Shape;
        return shape.Extents;
    }

    public void ForceJump()
    {
        machine.ForceState(STATE_JUMP);
    }

    public override bool OnHit(Bullet bullet)
    {
        Hurt(bullet.Damage, Math.Sign(bullet.Direction.x));

        return true;
    }

    public void Hurt(int damage, int direction = 0)
    {
        //Face the damage direction
        hurtDirection = direction;
        machine.ForceState(STATE_HURT);
        LoseHP(damage);
    }

    public void RecoverHP(int health)
    {
        var newHP = hp + health;
        if (newHP > 50)
        {
            hp = 50;
        }
        else
        {
            hp = newHP;
        }

    }

    #region  Player States

    public void OnIdleStart()
    {
        if (bodyAnimationPlayer != null)
        {
            bodyAnimationPlayer.Stop();
            bodyAnimationPlayer.Play("idle");
        }

        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HORIZONTAL_SPEED;
    }
    public FSM.State OnIdleUpdate(float delta)
    {
        Vector2 direction = InputManager.Direction;
        Movement.ApplyMotion(direction.x, direction.y);

        if (Movement.IsFalling())
            return STATE_FALL;

        if (InputManager.JumpPressed)
            return STATE_JUMP;

        if (InputManager.DashPressed)
            return STATE_DASH;

        if (direction.x != 0)
            return STATE_RUN;

        return STATE_IDLE;
    }

    public void OnRunStart()
    {
        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HORIZONTAL_SPEED;
        bodyAnimationPlayer.Stop();
        bodyAnimationPlayer.PlaybackSpeed = 2.0f;
        bodyAnimationPlayer.Play("run_normal");

        SoundManager.Instance.RunAudioPlayer.Play();
    }

    public void OnRunEnd()
    {
        bodyAnimationPlayer.Stop();
        SoundManager.Instance.RunAudioPlayer.Stop();
    }

    public FSM.State OnRunUpdate(float delta)
    {
        Vector2 direction = InputManager.Direction;
        Movement.ApplyMotion(direction.x, direction.y);

        if (InputManager.JumpPressed)
            return STATE_JUMP;

        if (direction.x == 0)
            return STATE_IDLE;

        if (Movement.IsFalling())
            return STATE_FALL;

        if (InputManager.DashPressed)
            return STATE_DASH;

        return STATE_RUN;
    }

    private bool jumpReleased = false;



    public void OnJumpStart()
    {
        bodyAnimationPlayer.Stop();
        bodyAnimationPlayer.Play("jump");

        SoundManager.Instance.JumpAudioPlayer.Play();

        Movement.Jump();
        jumpReleased = false;
    }

    public void OnFallStart()
    {
        bodyAnimationPlayer.Stop();
        bodyAnimationPlayer.Play("fall");


    }
    public void OnFallEnd()
    {
        SoundManager.Instance.FallGroundAudioPlayer.Play();
    }

    public void OnJumpEnded()
    {

    }

    public FSM.State OnJumpUpdate(float delta)
    {
        Vector2 direction = InputManager.Direction;
        Movement.ApplyMotion(direction.x, direction.y);

        if (!InputManager.JumpPressed)
            jumpReleased = true;

        if (jumpReleased)
            Movement.CapJumpSpeed();

        if (Movement.IsFalling())
            return STATE_FALL;

        if (Movement.IsGrounded)
            return STATE_IDLE;

        return STATE_JUMP;
    }

    public FSM.State OnFallUpdate(float delta)
    {
        Vector2 direction = InputManager.Direction;
        Movement.ApplyMotion(direction.x, direction.y);

        if (Movement.IsGrounded)
        {
            MoonHunter.Instance.AddMovementDust(feetPosition.GlobalPosition);
            return STATE_IDLE;
        }

        return STATE_FALL;
    }



    //Dash variables
    private float dashTime = 0;
    private int dashDirection;

    public void OnDashStart()
    {
        dashTime = MoonHunter.Constants.PLAYER_DASH_DURATION;
        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_DASH_SPEED;
        dashDirection = Movement.FacingDirection;
        MoonHunter.Instance.AddMovementDust(feetPosition.GlobalPosition);
        SoundManager.Instance.DashAudioPlayer.Play();
    }

    public void OnDashEnded()
    {
        dashTime = 0;
    }

    public FSM.State OnDashUpdate(float delta)
    {
        int dirX = (int)InputManager.Direction.x;
        dashTime -= delta;
        Movement.ApplyMotion(dashDirection);

        if (-1 * dirX == dashDirection)
            return STATE_IDLE;

        if (dashTime <= 0)
        {
            dashTime = 0;
            return STATE_IDLE;
        }

        if (InputManager.JumpPressed)
            return STATE_JUMP;

        return STATE_DASH;
    }

    private int hurtDirection;

    public void OnHurtEnter()
    {
        invincibleAnimPlayer.Play("Hurt");
        Vulnerable = false;

        SoundManager.Instance.HurtAudioPlayer.Play();

        // PlayAnimation(ANIMATION_BLINK);
        hurtPose.Visible = true;
        mesh.Visible = weapon.Visible = false;

        STATE_HURT.ResetTime();
        if (hurtDirection == 0)
            hurtDirection = -Movement.FacingDirection;

        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HURT_SPEED;
    }

    public FSM.State OnHurtUpdate(float delta)
    {
        Movement.ApplyNonFacingMotion(hurtDirection);

        if (STATE_HURT.IsOver)
            return STATE_IDLE;

        return STATE_HURT;
    }

    public void OnHurtExit()
    {
        hurtPose.Visible = false;
        mesh.Visible = weapon.Visible = true;
        PlayAnimation(ANIMATION_NORMAL);
        Movement.InputHorizonalSpeed = MoonHunter.Constants.PLAYER_HORIZONTAL_SPEED;


        invincibleAnimPlayer.Stop();
        body.Modulate = new Color(1, 1, 1, 1);
        invencibilityMachine.ForceState(STATE_HURT_INVINCIBLE);
    }

    public void OnHurtInvincibleEnter()
    {
        invincibleAnimPlayer.Play("Invincible");
        Vulnerable = false;
    }

    public void OnHurtInvincibleExit()
    {
        invincibleAnimPlayer.Play("None");
        // invincibleAnimPlayer.Stop();
        Vulnerable = true;
    }

    #endregion

    public void OnLootTouched(Node body)
    {
        Loot loot = (Loot)body;
        SoundManager.Instance.EnergyCoinAudioPlayer.Play();
        //Find loot type and ammount
        // MoonHunter.Instance.GameState.ClaimLoot(loot);

        // int prevHp = hp;

        // if (hp > MoonHunter.Constants.PLAYER_MAX_ENERGY)
        int newHp = hp + loot.Ammount;
        if (newHp > MoonHunter.Constants.PLAYER_MAX_ENERGY)
        {
            newHp = MoonHunter.Constants.PLAYER_MAX_ENERGY;
        }
        int gained = newHp - hp;
        hp += gained;
        // hp += loot.Ammount;
        MoonHunterState.LootCollected += gained;
        loot.QueueFree();
    }

    public override void Die()
    {
        machine.ForceState(STATE_NO_ACTION);
        // base.OnDied();
        // PlayAnimation(ANIMATION_DIE);
        Enabled = false;
        // machine.ForceState(STATE_NO_ACTION);
        MoonHunter.Instance.PlayerDied();
    }

    public void OnEnemyTouch(BaseEnemy enemy)
    {
        if (Vulnerable)
        {
            Hurt(enemy.TouchDamage);
        }
    }
}
