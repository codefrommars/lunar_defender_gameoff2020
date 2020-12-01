
using Godot;
using System;

public class DroneB : DroneA
{
    public override void Shoot()
    {
        Bullet bullet = ShootAt(BulletType, GlobalPosition, MoonHunter.Instance.Player.GlobalPosition);
        ConfigureBullet(bullet);

        bullet = ShootAt(BulletType, GlobalPosition, MoonHunter.Instance.Player.GlobalPosition + new Vector2(80, 80));
        ConfigureBullet(bullet);

        bullet = ShootAt(BulletType, GlobalPosition, MoonHunter.Instance.Player.GlobalPosition - new Vector2(80, 80));
        ConfigureBullet(bullet);
    }
}
