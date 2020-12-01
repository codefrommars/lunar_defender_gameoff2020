using Godot;
using System;

public class StartMessageStage : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
          new ShowFramedMessageAction("Reach the lab protecting the mining rig and destroying the invaders.", "Mission Brief")
        );
        return script;
    }
}
