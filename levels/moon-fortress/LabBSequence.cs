using Godot;
using System;

public class LabBSequence : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            // new ShowMessageAction("More enemies???", 3.0f),
            // new ShowMessageAction("Kill all enemies", new Vector2(0.5f, 0.1f)),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.5f,
                (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER)
            ),
            new WaitForEnemyWave(),
            // new ShowMessageAction("Good Job", 3.0f),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.5f,
                (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER)
            ),
            // new HideMessageAction(),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0),
            // new ScriptActionWait(0.5f),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1),
            // new ScriptActionWait(0.5f),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0),
            // new ScriptActionWait(0.5f),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1),
            // new ScriptActionWait(0.5f),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0),
            // new ScriptActionWait(0.5f),
            // new SpawnEnemyAction(WALKER, MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1),
            // new ScriptActionWait(0.5f),
            // new WaitForEnemyWave(),
            new WaitForEnemyWave(),
            // new ShowMessageAction("Good nice"),
            new SetLockStageTransitions(stage, false)
        );

        return script;
    }
}
