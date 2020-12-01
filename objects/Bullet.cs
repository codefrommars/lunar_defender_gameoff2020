

using Godot;

public class Bullet : KinematicBody2D
{
    private AnimatedSprite sprite;
    private CPUParticles2D muzzleParticles;

    public float Speed { get; set; } = 3000;
    public int Damage { get; set; } = 5;

    public Vector2 Direction { get; private set; }

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite>("Sprite");
        sprite.Playing = true;

        muzzleParticles = GetNode<CPUParticles2D>("MuzzleParticles");
        if (muzzleParticles != null)
            muzzleParticles.Emitting = true;
    }

    public void SetMask(uint mask)
    {
        CollisionMask = mask;
    }

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D collision = MoveAndCollide(Direction * Speed * delta, false);
        if (collision != null)
        {
            Object collider = collision.Collider;

            if (collider is HittablePart)
            {
                HittablePart part = (HittablePart)collider;
                if (part.OnHit(this))
                {
                    DidHit(part, collision);
                    return;
                }
            }

            if (collider is Box)
            {
                Box box = (Box)collider;
                box.DoDamage(5);
            }

            DisintegrateBullet(collision);
        }
    }

    public void SetBulletRotation(float rotation)
    {
        Rotation = rotation;
        Direction = Mathf.Polar2Cartesian(1.0f, Rotation);
    }

    public void DidHit(HittablePart part, KinematicCollision2D collision)
    {
        SoundManager.Instance.HitEnemiesAudioPlayer.Play();
        QueueFree();
    }

    public void DisintegrateBullet(KinematicCollision2D collision)
    {
        MoonHunter.Instance.AddBulletExplosion(collision.Position, Rotation);
        SoundManager.Instance.HitLevelAudioPlayer.Play();
        QueueFree();
    }

}
