

using Godot;

public class Jumper : BaseEnemy
{
    public float WaitingDuration { get; set; } = 2.0f;

    protected WaitState STATE_BEHAVIOR_WAIT_FOR_JUMP;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    protected override FSM BuildStateMachine()
    {
        STATE_BEHAVIOR_WAIT_FOR_JUMP = new WaitState(WaitingDuration);
        STATE_BEHAVIOR_WAIT_FOR_JUMP.OnExit = OnWaitForJumpEnded;
        return new FSM(STATE_BEHAVIOR_WAIT_FOR_JUMP);
    }

    #region Default
    public void OnWaitForJumpEnded()
    {
        Movement.Jump();
        // STATE_BEHAVIOR_WAIT_FOR_JUMP.Reset(WaitingDuration);
    }
    #endregion

}
