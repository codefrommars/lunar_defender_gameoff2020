
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
        SoundManager.Instance.PlayRandomExplosion();

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
    }

    public virtual void SetParameters(params object[] parameters)
    {

    }

    protected virtual void ConfigureBullet(Bullet bullet)
    {
        bullet.Damage = MoonHunter.Constants.DEFAULT_BULLET_ENEMY_DAMAGE;
        bullet.Speed = 500;
    }

}