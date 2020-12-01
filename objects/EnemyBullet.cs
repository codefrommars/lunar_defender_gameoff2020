using Godot;
using System;

public class EnemyBullet : Bullet
{
    public override void _Ready()
    {
        SoundManager.Instance.EnemyShootAudioPlayer.Play();
    }
}
