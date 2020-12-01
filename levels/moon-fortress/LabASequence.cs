
using Godot;
using System;

public class LabASequence : StageScript
{

    protected override ScriptAction DoBuildFor(Stage stage)
    {
        Rect2 FlyingArea1 = new Rect2(0.2f, 0.2f, 0.6f, 0.3f);
        ScriptActionSequence script = new ScriptActionSequence(false,
            new SetLockStageTransitions(stage, true),
            // new ShowMessageAction("Ready for first wave", 3.0f),

            // new ShowMessageAction("Kill all invaders", new Vector2(0.5f, 0.1f)),
            // //new ShowMessageAction("Do you understand?", new Vector2(0.5f, 0.1f)),
            // //new StopConfirmAction("ui_accept"),
            new HideMessageAction(),
             new SpawnSingleEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_DROP,
                CAMPER, MoonHunter.Constants.OTHER_ENEMIES_GROUP, new Vector2(0.2f, 0.4f)),

            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.5f,
                (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER)
            ),

            new WaitForEnemyWave(true,
                new SpawnUntilKilledAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_A, FlyingArea1),
                new WaitScriptAction(2.0f)
            ),
            // new ShowMessageAction("Good Job", 3.0f),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.25f,
                (0, SAW), (1, SAW), (0, SAW), (1, SAW), (0, WALKER), (1, WALKER)
            ),
            new WaitForEnemyWave(false,
                new SpawnSingleEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_DROP,
                SAW_TROOPER, MoonHunter.Constants.OTHER_ENEMIES_GROUP, new Vector2(0.1f, 0.1f))
            ),
            // new ShowMessageAction("Good Job", 3.0f),
            new SpawnEnemyAction(MoonHunter.Constants.TARGET_ENEMIES_GROUP, 0.5f,
                (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER), (0, WALKER), (1, WALKER)
            ),
            new WaitForEnemyWave(false,
                new SpawnSingleEnemyAction(MoonHunter.Constants.OTHER_ENEMIES_GROUP, 2, DRONE_DROP,
                SAW_TROOPER, MoonHunter.Constants.OTHER_ENEMIES_GROUP, new Vector2(0.1f, 0.1f))
            ),
            // new ShowMessageAction("Level completed, go to the next room"),
            new SetLockStageTransitions(stage, false)
        );

        return script;
    }

}
