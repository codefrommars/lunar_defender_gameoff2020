using Godot;
using System;

public class LearnBlocks : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.25f,
                (0, WALKER), (1, WALKER), (2, WALKER), (3, SAW)
            ),
            new ShowFramedMessageAction("The Material collector gets damaged when it's touched by an enemy. If it's destroyed the mission will be failed.", "Warning"),
            new WaitForEnemyWave(),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
                (3, WALKER), (3, WALKER), (3, WALKER), (3, SAW), (3, SAW), (3, SAW)
            ),
            new WaitForEnemyWave(),
            new ShowFramedMessageAction("Press W [XBOX Y] to create an Energy Block. It can block enemies but costs energy", "Blocks"),
            new UnlockPowerupAction(stage, MoonHunterState.Powerup.Blocks),
            new SetLockStageTransitions(stage, false)
        );

        return script;
    }
}
