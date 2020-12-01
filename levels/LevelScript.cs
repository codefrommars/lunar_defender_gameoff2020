using Godot;

public class ScriptAction
{
    public virtual bool Act(float delta)
    {
        return true;
    }

    public bool CompleteIf(bool condition)
    {
        if (!condition)
            return false;

        return Complete();
    }

    public bool Complete()
    {
        Reset();
        return true;
    }

    public virtual void Reset()
    {

    }
}

public class ScriptActionSequence : ScriptAction
{
    private ScriptAction[] actions;
    private bool loop = false;
    private int current;


    public ScriptActionSequence(bool loop = false, params ScriptAction[] list)
    {
        actions = list;
        this.loop = loop;
    }

    public override void Reset()
    {
        current = 0;
    }

    public override bool Act(float delta)
    {
        while (current < actions.Length)
        {
            if (!actions[current].Act(delta))
                return false;

            actions[current].Reset();
            current++;
        }

        if (loop)
        {
            Reset();
            return false;
        }

        return true;
    }
}

public class ScriptActionParallel : ScriptAction
{
    protected ScriptAction[] actions;
    protected int running;

    public override void Reset()
    {
        running = actions.Length;
    }

    public ScriptActionParallel(params ScriptAction[] list)
    {
        actions = list;
    }

    public override bool Act(float delta)
    {
        for (int i = running - 1; i >= 0; i--)
        {
            ScriptAction action = actions[i];
            if (action.Act(delta))
            {
                action.Reset();

                running--;
                if (running != i)
                {
                    actions[i] = actions[running];
                    actions[running] = action;
                }
            }
        }

        return CompleteIf(running == 0);
    }
}

public class WaitScriptAction : ScriptAction
{
    private float duration;
    private float timeLeft;

    public WaitScriptAction(float duration)
    {
        this.duration = duration;
        timeLeft = duration;
    }

    public override void Reset()
    {
        timeLeft = duration;
    }

    public override bool Act(float delta)
    {
        timeLeft -= delta;
        return CompleteIf(timeLeft < 0);
    }
}

public struct SpawnEnemyInfo
{
    public int SpawnerIndex { get; set; }
    public string EnemyType { get; set; }

    public SpawnEnemyInfo(int index, string type)
    {
        SpawnerIndex = index;
        EnemyType = type;
    }
}

public class SpawnEnemyAction : ScriptAction
{
    private (int, string)[] infos;
    private string group;
    private float timeToWait;

    private float currTimeToWait;
    private int current;

    public SpawnEnemyAction(string group, float timeToWait, params (int, string)[] infos)
    {
        this.group = group;
        this.infos = infos;
        this.timeToWait = this.currTimeToWait = timeToWait;
        this.current = 0;
    }

    public override void Reset()
    {
        this.currTimeToWait = timeToWait;
        this.current = 0;
    }

    public override bool Act(float delta)
    {
        if (WaitForNext(delta))
            return CompleteIf(SpawnCurrent());

        return false;
    }

    private bool SpawnCurrent()
    {
        (int, string) info = infos[current];
        MoonHunter.Instance.SpawnEnemyAtSpawner(info.Item1, info.Item2, group);
        current++;
        currTimeToWait = timeToWait;
        return current >= infos.Length;
    }

    private bool WaitForNext(float delta)
    {
        currTimeToWait -= delta;
        return currTimeToWait <= 0;
    }
}

public class SpawnSingleEnemyAction : ScriptAction
{
    private string group;
    private int spawnerIndex;
    private string enemyType;
    private object[] parameters;

    public SpawnSingleEnemyAction(string group, int spawnerIndex, string enemyType, params object[] parameters)
    {
        this.group = group;
        this.spawnerIndex = spawnerIndex;
        this.enemyType = enemyType;
        this.parameters = parameters;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.SpawnEnemyAtSpawner(spawnerIndex, enemyType, group, parameters);

        return Complete();
    }
}

public class DropEnemyAction : ScriptAction
{
    private int dropPlace;

    private string group;
    private string enemyType;

    private object[] parameters;

    public DropEnemyAction(int dropPlace, string group, string enemyType, params object[] parameters)
    {
        this.dropPlace = dropPlace;
        this.group = group;
        this.enemyType = enemyType;
        this.parameters = parameters;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.DropEnemy(dropPlace, enemyType, group, parameters);
        return Complete();
    }
}
public class DropEnemyUntilKilledAction : ScriptAction
{
    private int dropPlace;

    private string group;
    private string enemyType;

    private object[] parameters;

    private BaseEnemy enemy;

    public DropEnemyUntilKilledAction(int dropPlace, string group, string enemyType, params object[] parameters)
    {
        this.dropPlace = dropPlace;
        this.group = group;
        this.enemyType = enemyType;
        this.parameters = parameters;
    }

    public override void Reset()
    {
        enemy = null;
    }

    public override bool Act(float delta)
    {
        if (enemy == null)
        {
            DroneDrop drone = (DroneDrop)MoonHunter.Instance.DropEnemy(dropPlace, enemyType, group, parameters);
            enemy = drone.DropEnemy;
        }

        return CompleteIf(!enemy.Alive);
    }
}

public class WaitForEnemyWave : ScriptActionSequence
{

    public WaitForEnemyWave(bool loop = false, params ScriptAction[] list) : base(loop, list) { }

    public override bool Act(float delta)
    {
        base.Act(delta);
        return CompleteIf(MoonHunter.Instance.GetTargetEnemiesCount() <= 0);
    }
}


public class SpawnUntilKilledAction : ScriptAction
{
    private string group;
    private int spawnerIndex;
    private string enemyType;
    private object[] parameters;

    private BaseEnemy enemy;

    public SpawnUntilKilledAction(string group, int spawnerIndex, string enemyType, params object[] parameters)
    {
        this.group = group;
        this.spawnerIndex = spawnerIndex;
        this.enemyType = enemyType;
        this.parameters = parameters;
    }

    public override void Reset()
    {
        enemy = null;
    }


    public override bool Act(float delta)
    {
        if (enemy == null)
            enemy = MoonHunter.Instance.SpawnEnemyAtSpawner(spawnerIndex, enemyType, group, parameters);

        return CompleteIf(!enemy.Alive);
    }
}

public class ShowMessageAction : ScriptAction
{
    private string message;
    private float duration;
    private Vector2 position;
    private bool infinite = false;
    private float currTimeLeft;

    public ShowMessageAction(string message, Vector2 position, float duration = -1)
    {
        this.message = message;
        this.duration = duration;
        this.position = position;
        infinite = duration < 0;
        this.currTimeLeft = duration;
    }

    public ShowMessageAction(string message, float duration = -1)
        : this(message, new Vector2(0.5f, 0.5f), duration)
    {
    }

    public override void Reset()
    {
        this.currTimeLeft = duration;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.ShowMessage(message, position);

        if (infinite)
            return Complete();

        currTimeLeft -= delta;
        if (currTimeLeft <= 0)
        {
            MoonHunter.Instance.HideMessage();
        }

        return CompleteIf(currTimeLeft <= 0);
    }
}

public class HideMessageAction : ScriptAction
{
    public override bool Act(float delta)
    {
        MoonHunter.Instance.HideMessage();
        return Complete();
    }
}

public class SetLockStageTransitions : ScriptAction
{
    private bool locked;
    private Stage stage;

    public SetLockStageTransitions(Stage stage, bool locked)
    {
        this.locked = locked;
        this.stage = stage;
    }

    public override bool Act(float delta)
    {
        stage.SetTransitionsLock(locked);
        return Complete();
    }
}

public class StopConfirmAction : ScriptAction
{
    private string action;

    public StopConfirmAction(string action)
    {
        this.action = action;
    }

    public override bool Act(float delta)
    {
        return CompleteIf(Input.IsActionJustPressed(action));
    }
}

public class EmitSignalAction : ScriptAction
{
    private Node emitter;
    private string signal;
    private object[] parameters;

    public EmitSignalAction(Node emitter, string signal, params object[] parameters)
    {
        this.emitter = emitter;
        this.signal = signal;
        this.parameters = parameters;
    }

    public override bool Act(float delta)
    {
        this.emitter.EmitSignal(this.signal, parameters);
        return Complete();
    }
}

public class SaveCheckPointAction : ScriptAction
{
    private Stage stage;
    public SaveCheckPointAction(Stage stage)
    {
        this.stage = stage;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.DoCheckPoint(stage);

        return Complete();
    }
}

public class ShowFramedMessageAction : ScriptAction
{
    string content, title;

    public ShowFramedMessageAction(string content, string title = "Message")
    {
        this.content = content;
        this.title = title;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.ShowFramedMessage(content, title);
        return Complete();
    }
}

public class ShowEndGameMessageAction : ScriptAction
{
    public ShowEndGameMessageAction()
    {
    }

    public override bool Act(float delta)
    {

        MoonHunter.Instance.ShowFramedMessage(MoonHunter.Instance.GameState.GetMissionCompleted(), "Mission Completed");
        return Complete();
    }
}

public class UnlockPowerupAction : ScriptAction
{
    private Stage stage;
    MoonHunterState.Powerup powerup;

    public UnlockPowerupAction(Stage stage, MoonHunterState.Powerup powerup)
    {
        this.powerup = powerup;
        this.stage = stage;
    }

    public override bool Act(float delta)
    {
        MoonHunter.Instance.UnlockPowerup(this.powerup);
        MoonHunter.Instance.DoCheckPoint(stage);
        return Complete();
    }
}