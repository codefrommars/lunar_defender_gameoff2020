
using Godot;
using System;

public class Checkpoint : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
          new SaveCheckPointAction(stage)
        );
        return script;
    }
}
