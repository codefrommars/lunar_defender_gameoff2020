using Godot;

public class LevelScriptRunner : Node
{
    public ScriptAction Script { get; set; }

    public override void _PhysicsProcess(float delta)
    {

        if (Script.Act(delta))
        {
            //It's done. Send a signal or something ??
            OnCompleted();
            return;
        }
    }

    protected void OnCompleted()
    {
        QueueFree();
    }
}
