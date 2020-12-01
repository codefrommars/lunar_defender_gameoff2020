using Godot;

public class BaseLevel : Node2D
{
    protected Node2D stages;
    public Position2D PlayerStart { get; private set; }

    public override void _Ready()
    {
        stages = GetNode<Node2D>("Stages");
        PlayerStart = GetNode<Position2D>("PlayerStart");
    }

    private Stage GetStageAt(Vector2 globalPosition)
    {
        for (int i = 0; i < stages.GetChildCount(); i++)
        {
            Stage stage = stages.GetChildOrNull<Stage>(i);
            Rect2 global = stage.GetGlobalRect();

            if (global.HasPoint(globalPosition))
                return stage;
        }

        return null;
    }

    public Stage GetStage(string name)
    {
        return stages.GetNode<Stage>(name);
    }

    public Stage GetInitialStage()
    {
        return GetStageAt(PlayerStart.GlobalPosition);//stages.GetChild<Stage>(0);
    }
}
