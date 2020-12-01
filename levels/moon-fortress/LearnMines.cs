using Godot;
using System;

public class LearnMines : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        Rect2 FlyingArea1 = new Rect2(0.2f, 0.2f, 0.6f, 0.3f);

        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.001f,
                (2, SAW_TROOPER), (3, SAW_TROOPER)
            ),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2.0f,
                (0, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (0, WALKER), (1, WALKER)
            ),
            new WaitForEnemyWave(),

            new SpawnUntilKilledAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 4, DRONE_A, FlyingArea1),

            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
                (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (1, SAW), (0, WALKER), (1, SAW)
            ),
            new WaitForEnemyWave(),
            new SpawnSingleEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
                (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW)
            ),
            new WaitForEnemyWave(),
            new SpawnSingleEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            new WaitScriptAction(4.0f),
            new SpawnSingleEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
                (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER)
            ),
            new WaitForEnemyWave(),
            new ShowFramedMessageAction("Press E [XBOX L] to place a mine. Mines deal damage to enemies passing by, but consumes your energy. You can only place a maximum of 5 at a time.", "Mines"),
            new UnlockPowerupAction(stage, MoonHunterState.Powerup.Mines),
            new SetLockStageTransitions(stage, false)
        );
        return script;
    }
}