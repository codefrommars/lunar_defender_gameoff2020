
using Godot;
using System;

public class DefeatCamper : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        Rect2 FlyingArea1 = new Rect2(0.2f, 0.2f, 0.6f, 0.3f);

        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            //new DropEnemyAction(0, MoonHunter.Constants.OTHER_ENEMIES_GROUP, SAW_TROOPER),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.001f,
                (1, CAMPER), (0, SAW_TROOPER), (2, SAW_TROOPER)
            ),
            new WaitForEnemyWave(true,
                new DropEnemyUntilKilledAction(0, MoonHunter.Constants.OTHER_ENEMIES_GROUP, SAW_TROOPER),
                new WaitScriptAction(2.0f)
            //new SpawnUntilKilledAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 4, DRONE_A, FlyingArea1),
            ),
            // new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 2.0f,
            //     (0, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (0, WALKER), (1, WALKER)
            // ),

            // new SpawnUntilKilledAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 4, DRONE_A, FlyingArea1),

            // new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
            //     (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (1, SAW), (0, WALKER), (1, SAW)
            // ),
            // new WaitForEnemyWave(),
            // new SpawnSingleEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
            // new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 1.0f,
            //     (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW), (0, WALKER), (1, SAW)
            // ),
            new WaitForEnemyWave(),
            new SetLockStageTransitions(stage, false)
        );
        return script;
    }
}