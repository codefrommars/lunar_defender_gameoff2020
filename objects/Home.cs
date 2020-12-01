using Godot;
using System;

public class Home : Node2D
{
    private int hp = MoonHunter.Constants.MACHINE_MAX_ENERGY;
    private Label label;

    public int Energy { get { return hp; } }

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        label.Text = hp + "";
    }

    public void OnEnemyEntered(Node body)
    {
        if (body is BaseEnemy)
        {
            BaseEnemy enemy = (BaseEnemy)body;
            enemy.OnBaseEntered();
            CauseDamage(10);
        }
    }

    private void CauseDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            MoonHunter.Instance.PlayerDied();
        }
    }
}
