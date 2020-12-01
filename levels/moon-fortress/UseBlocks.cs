using Godot;
using System;

public class UseBlocks : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            //new SetLockStageTransitions(stage, true),
            //new ShowMessageAction("Defend the Material Collector", new Vector2(0.5f, 0.1f)),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.01f,
                (0, SAW_TROOPER), (1, SAW_TROOPER)
            )
        //new SetLockStageTransitions(stage, false)
        );

        return script;
    }
}
