using Godot;
using System;

public class UseBlocks : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.01f,
                (0, SAW_TROOPER), (1, SAW_TROOPER)
            )
        );

        return script;
    }
}
