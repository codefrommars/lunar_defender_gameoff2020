using Godot;

[Tool]
public class StageTransition : Area2D
{
    private CollisionShape2D collisionShape;
    private Position2D playerHandler;

    public Vector2 Direction { get; private set; }
    public Margin TransitionMargin { get; private set; }

    public Rect2 TargetView { get; set; }
    public Stage Stage { get; private set; }

    public bool Locked { get; set; }

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        playerHandler = GetNode<Position2D>("PlayerHandler");

        Direction = Mathf.Polar2Cartesian(1.0f, Rotation).Round().Sign();

        if (Direction.x == 1)
        {
            TransitionMargin = Margin.Left;
        }
        else if (Direction.x == -1)
        {
            TransitionMargin = Margin.Right;
        }
        else if (Direction.y == 1)
        {
            TransitionMargin = Margin.Bottom;
        }
        else
            TransitionMargin = Margin.Top;

        Stage = GetNode<Stage>("../..");
    }

    public bool Active
    {
        set
        {
            collisionShape.Disabled = !value;
        }
    }


    public void OnPlayerEntered(Node body)
    {
        if (Locked)
            return;

        MoonHunter.Instance.TriggerTransition(this);
    }

    public void OnPlayerExited(Node body)
    {

    }

    public string GetNextStageName()
    {
        return Name;
    }

    public Vector2 GetTransitionPosition(Hunter hunter)
    {
        Vector2 extents = hunter.GetCollisionExtents();
        Vector2 position = hunter.GlobalPosition;
        Vector2 delta = (playerHandler.GlobalPosition - hunter.GlobalPosition) * Direction.Abs() + extents * Direction;

        return position + delta;
    }

    public StageTransition GetMirror(Stage stage)
    {
        return stage.GetTransition(Stage.Name);
    }

}
