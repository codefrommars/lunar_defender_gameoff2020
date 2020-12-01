using Godot;
using System;

public class Box : KinematicBody2D
{

    private Label label;
    private int hp = 30;
    const float gravity = 1000.0f;
    private Vector2 velocity;
    private Timer areaReset;

    private Area2D hitbox;

    private AnimationPlayer animationPlayer;
    private Sprite blockSprite;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        hitbox = GetNode<Area2D>("Hitbox");
        areaReset = GetNode<Timer>("AreaReset");
        blockSprite = GetNode<Sprite>("Body/Mesh/block");

        animationPlayer = GetNode<AnimationPlayer>("Body/Mesh/AnimationPlayer");
        animationPlayer.Play("idle");
    }

    public override void _PhysicsProcess(float delta)
    {
        label.Text = hp + "";
        velocity.y += delta * gravity;
        velocity = MoveAndSlideWithSnap(velocity, new Vector2(60, 60), Vector2.Up, true, 2, 0.785398f, false);
    }

    public void OnEnemyBodyEntered(Node body)
    {
        if (body is BaseEnemy)
        {
            DoDamage(10);
        }

        if (body is Bullet)
        {
            DoDamage(5);
        }
    }

    public void DoDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            MoonHunter.Instance.AddExplosion(GlobalPosition);
            QueueFree();
            return;
        }

        CallDeferred(nameof(SetMonitoring), false);
        areaReset.WaitTime = 0.75f;
        areaReset.Start();

        HurtAnimation(0.1f);
    }

    public void HurtAnimation(float duration = 0.05f)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(duration);
        timer.Connect("timeout", this, nameof(ResetMaterial));
        blockSprite.Modulate = new Color(1, 0.41f, 0.41f, 1);
    }

    private void ResetMaterial()
    {
        blockSprite.Modulate = new Color(1, 1, 1, 1);
    }

    private void SetMonitoring(bool value)
    {
        hitbox.Monitoring = value;
    }

    public void OnAreaReset()
    {
        CallDeferred(nameof(SetMonitoring), true);
    }
}
