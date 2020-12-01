
using Godot;
using System;

public class DefeatAllRoom : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.001f,
                (0, WALKER), (1, WALKER), (2, WALKER), (3, SAW_TROOPER)
            ),
            new WaitForEnemyWave(),
            new SetLockStageTransitions(stage, false)
        );
        return script;
    }
}
