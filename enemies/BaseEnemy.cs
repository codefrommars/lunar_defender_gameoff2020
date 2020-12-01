
using Godot;

public class BaseEnemy : Character
{
    [Export]
    protected PackedScene BulletType;

    private bool atSpawn = true;

    public int TouchDamage { get; set; } = MoonHunter.Constants.TOUCH_DAMAGE;
    private Vector2 spawnPosition;

    public override void _Ready()
    {
        base._Ready();

        spawnPosition = GlobalPosition;
        atSpawn = true;
    }

    public void SetFacingDirection(int dir)
    {
        body.Scale *= new Vector2(dir, 1);
        Movement.Horizontal = dir;
    }

    public virtual void OnBaseEntered()
    {
        MoonHunter.Instance.AddExplosion(GlobalPosition);
        OnDissapeared();
    }

    public void OnPlayerDamageAreaEntered(Node body)
    {
        if (body is Hunter)
        {
            ((Hunter)body).OnEnemyTouch(this);
        }
    }

    public override void Die()
    {
        base.Die();
        MoonHunter.Instance.AddExplosion(GlobalPosition);
        MoonHunter.Instance.AddLoot(GlobalPosition, MoonHunter.Constants.DEFAULT_LOOT, 5);
        //
        SoundManager.Instance.PlayRandomExplosion(); //Explosion0AudioPlayer.Play();

        MoonHunterState.EnemiesDestroyed += 1;
        OnDissapeared();
    }

    public Bullet ShootAt(PackedScene bulletType, Vector2 from, float angle)
    {
        Bullet bullet = MoonHunter.Instance.NewProjectile<Bullet>(bulletType);
        bullet.Position = from;

        bullet.SetBulletRotation(angle);

        return bullet;
    }


    public Bullet ShootAt(PackedScene bulletType, Vector2 position, Vector2 target)
    {
        Vector2 direction = GlobalPosition.DirectionTo(target);
        return ShootAt(bulletType, position, direction.Angle());
        // Vector2 direction = GlobalPosition.DirectionTo(target);

        // Bullet bullet = MoonHunter.Instance.NewProjectile<Bullet>(bulletType);
        // bullet.Position = position;

        // bullet.SetBulletRotation(direction.Angle());

        // return bullet;

    }

    public virtual void SetParameters(params object[] parameters)
    {

    }

    protected virtual void ConfigureBullet(Bullet bullet)
    {
        bullet.Damage = MoonHunter.Constants.DEFAULT_BULLET_ENEMY_DAMAGE;
        bullet.Speed = 500;
    }

    // public override bool OnHit(Bullet bullet)
    // {
    //     if (!Vulnerable)
    //         return false;

    //     // bullet.DidHit();
    //     return base.OnHit(bullet);
    // }

    // public void OnBodyEntered(Node body)
    // {
    //     // GD.Print("Hit !: " + body);
    //     if (body is Bullet)
    //     {
    //         OnHit((Bullet)body);
    //         return;
    //     }

    //     if (body is KinematicBody2D)
    //     {
    //         GD.Print("Hurt player: " + body);
    //     }
    // }
    // public override void _PhysicsProcess(float delta)
    // {
    //     bool isInBounds = IsInCameraBounds(GlobalPosition);

    //     if (!isInBounds)
    //     {
    //         if (!atSpawn)
    //         {
    //             bool spawnInBounds = IsInCameraBounds(spawnPosition);
    //             if (!spawnInBounds)
    //             {
    //                 GlobalPosition = spawnPosition;
    //                 atSpawn = true;
    //                 SetInitialState();
    //             }
    //         }
    //     }
    //     else
    //     {
    //         base._PhysicsProcess(delta);
    //         atSpawn = false;
    //     }

    // }

    // protected virtual void SetInitialState()
    // {

    // }

    // protected bool IsInCameraBounds(Vector2 position)
    // {
    //     Rect2 visibleRect = MoonHunter.Instance.Camera.GetVisibleRect();
    //     visibleRect.Size *= new Vector2(1.25f, 1.0f);

    //     return visibleRect.HasPoint(position);
    // }


}