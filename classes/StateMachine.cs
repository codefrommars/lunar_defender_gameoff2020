

public delegate void TransitionCallback();
public delegate FSM.State UpdateCallback(float delta);

public class FSM
{
    public class State
    {
        public TransitionCallback OnEnter { get; set; }
        public TransitionCallback OnExit { get; set; }
        public UpdateCallback OnUpdate { get; set; }

        public string Name { get; set; }

        public State(UpdateCallback onUpdate = null, TransitionCallback onEnter = null, TransitionCallback onExit = null)
        {
            this.OnUpdate = onUpdate;
            this.OnEnter = onEnter;
            this.OnExit = onExit;
        }

        public FSM.State Enter()
        {
            OnEnter?.Invoke();
            return this;
        }

        public FSM.State Exit()
        {
            OnExit?.Invoke();
            return this;
        }

        public State Update(float delta)
        {
            if (OnUpdate == null)
                return this;

            State next = OnUpdate.Invoke(delta);

            if (next != this)
                next = TransitionTo(next);

            return next;
        }

        public State TransitionTo(State next)
        {
            Exit();
            next?.Enter();
            return next;
        }
    }

    public State CurrentState { get; private set; }

    public FSM(State state)
    {
        ForceState(state);
    }

    public void Update(float delta)
    {
        CurrentState = CurrentState.Update(delta);
    }

    public void ForceState(State state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }
}

public class TimedState : FSM.State
{
    public float Duration { get; set; }
    protected UpdateCallback DelegatedUpdate { get; private set; }
    protected float totalTime = 0;

    public bool IsOver
    {
        get { return totalTime >= Duration; }
    }

    public TimedState(float duration, UpdateCallback delegatedUpdate)
    {
        Duration = duration;
        OnUpdate = OnTimedUpdated;
        DelegatedUpdate = delegatedUpdate;
    }

    public void ResetTime()
    {
        totalTime = 0;
    }

    public FSM.State OnTimedUpdated(float delta)
    {
        totalTime += delta;
        return DelegatedUpdate.Invoke(delta);
    }
}

public class EpsilonState : FSM.State
{
    public FSM.State NextState { get; set; }

    public EpsilonState(TransitionCallback callback, FSM.State nextState)
    {
        OnEnter = callback;
        NextState = nextState;
        this.OnUpdate = UpdateEpsilonState;
    }

    FSM.State UpdateEpsilonState(float delta)
    {
        return NextState;
    }
}

public class WaitState : FSM.State
{
    public int Times { get; set; } = 0;

    public float Duration { get; set; }
    public FSM.State NextState { get; set; }
    public UpdateCallback ConditionChecker { get; set; }
    public TransitionCallback OnReenterCallback;

    private float time;
    private int cycles;

    public WaitState(float duration = 1.0f, FSM.State nextState = null)
    {
        this.OnUpdate = UpdateWaitState;
        this.Duration = duration;

        if (nextState == null)
            nextState = this;

        this.NextState = nextState;
    }

    private FSM.State OnTimeUp()
    {
        time = 0;//Automatic reset
        cycles++;
        if (cycles < Times)//completed
        {
            OnReenterCallback?.Invoke();
            return this;
        }

        if (NextState == this && OnReenterCallback != null)
            OnReenterCallback();

        return NextState;
    }

    FSM.State UpdateWaitState(float delta)
    {
        time += delta;

        if (time >= Duration)
        {
            return OnTimeUp();
        }

        if (ConditionChecker == null)
            return this;

        return ConditionChecker.Invoke(delta);
    }
}
