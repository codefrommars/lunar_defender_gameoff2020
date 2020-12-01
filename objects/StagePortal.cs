
using Godot;

public class StagePortal : Area2D
{
    private CollisionShape2D collisionShape;
    private Position2D playerHandler;

    public Vector2 Direction { get; private set; }
    public Margin TransitionMargin { get; private set; }

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

    }

    public void OnPlayerExited(Node body)
    {
    }

    public string GetNextStageName()
    {
        return Name;
    }

    public string GetCurrentStageName()
    {
        return Owner.Filename.GetFile().BaseName();
    }

    public Vector2 GetTransitionPosition(Hunter hunter)
    {
        Vector2 extents = hunter.GetCollisionExtents();
        Vector2 position = hunter.GlobalPosition;
        Vector2 delta = (playerHandler.GlobalPosition - hunter.GlobalPosition) * Direction.Abs() + extents * Direction;

        return position + delta;
    }

    public StagePortal GetMirror(BaseStage nextStage)
    {
        return nextStage.GetPortal(GetCurrentStageName());
    }
}
