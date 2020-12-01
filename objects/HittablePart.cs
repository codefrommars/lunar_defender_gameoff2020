using Godot;

public class HittablePart : StaticBody2D
{
    private Character owner;
    public bool Vulnerable { get; set; } = true;

    public override void _Ready()
    {
        owner = GetOwner<Character>();
    }

    public bool OnHit(Bullet bullet)
    {
        if (!Vulnerable || !owner.Vulnerable)
            return false;

        // bullet.DidHit(this);
        return owner.OnHit(bullet);
    }

    public void DoDamage(int damage, Vector2 dir = new Vector2())
    {
        if (!Vulnerable || !owner.Vulnerable)
            return;

        owner.DoDamage(damage, dir);
    }
}
