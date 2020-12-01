using Godot;
using System;

public class LearnLaser : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        Rect2 FlyingArea1 = new Rect2(0.3f, 0.2f, 0.4f, 0.3f);

        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.001f,
                (0, CAMPER), (2, SAW_TROOPER)
            ),
            new WaitScriptAction(5.0f),
            new SpawnEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 1.0f,
                (1, SAW), (1, WALKER), (1, SAW), (1, WALKER), (1, SAW), (1, WALKER)
            ),
            new WaitForEnemyWave(true,
                new SpawnUntilKilledAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1, DRONE_A, FlyingArea1),
                new WaitScriptAction(2.0f)
            ),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
                (1, WALKER), (1, WALKER), (1, WALKER), (1, WALKER), (1, WALKER), (1, WALKER)
            ),
            new SpawnSingleEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            new WaitScriptAction(2.0f),
            new SpawnSingleEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            new WaitForEnemyWave(true,
                new SpawnUntilKilledAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1, DRONE_A, FlyingArea1),
                new WaitScriptAction(2.0f)
            ),
            new ShowFramedMessageAction("Press F [XBOX R] to shoot a Beam of plasma. The Beam can go through walls, but consumes your energy.", "Plasma Beam"),
            new UnlockPowerupAction(stage, MoonHunterState.Powerup.Beam),
            new SetLockStageTransitions(stage, false)
        );
        return script;
    }
}