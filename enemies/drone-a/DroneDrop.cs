
using Godot;
using System;

public class DroneDrop : BaseEnemy
{
    private WaitState STATE_MOVING_TO, STATE_LEAVE, STATE_WAIT, STATE_DESTROY;
    private Tween moveTween;
    private float floatingTime;

    //
    private float dropFlyingDuration = 10.0f;
    private float leaveDuration = 5.0f;
    private float interstateDuration = 1.0f;

    //Parameters
    public Vector2 DropPlace { get; set; }
    public string DropGroup { get; set; }
    public string DropType { get; set; }

    private Node2D loadSlot;

    public BaseEnemy DropEnemy { get; private set; }

    public override void _Ready()
    {
        loadSlot = GetNode<Node2D>("Body/LoadSlot");

        moveTween = new Tween();
        AddChild(moveTween);

        base._Ready();

        hp = 50;

        DropEnemy = MoonHunter.Instance.SimpleSpawnEnemy(DropType, DropGroup);
        DropEnemy.Enabled = false;
        MoonHunter.Instance.RemoveEnemyFromEntities(DropEnemy);

        loadSlot.AddChild(DropEnemy);
    }

    protected override FSM BuildStateMachine()
    {
        STATE_DESTROY = new WaitState(leaveDuration)
        {
            OnReenterCallback = QueueFree
        };

        STATE_LEAVE = new WaitState(interstateDuration, STATE_DESTROY)
        {
            OnEnter = OnLeaveEnter,
            OnExit = OnLeaveExit
        };

        STATE_WAIT = new WaitState(interstateDuration, STATE_LEAVE);

        STATE_MOVING_TO = new WaitState(dropFlyingDuration, STATE_WAIT)
        {
            OnEnter = OnMovingToEnter,
            OnExit = OnMovingToExit
            // OnReenterCallback = OnMovingToExit
        };


        return new FSM(STATE_NO_ACTION);
    }

    public void MoveTo(Vector2 target, float duration)
    {
        //Vector2 target = MoonHunter.Instance.RelativeToGlobal(percentage);
        // Vector2 target = position;
        moveTween.InterpolateProperty(this, "position", Position, target, duration);
        moveTween.Start();
    }

    public override void ProcessMovement(float delta)
    {
        AddFloating(delta);
    }

    private void AddFloating(float delta)
    {
        floatingTime += delta;
        float y = (float)(20 * Math.Sin(floatingTime * 2 * Math.PI));
        body.Position = new Vector2(0, y);

    }

    public void OnMovingToEnter()
    {

        MoveTo(DropPlace, dropFlyingDuration);
    }

    public void OnMovingToExit()
    {

    }

    public void OnLeaveEnter()
    {
        Drop();

    }

    public void OnLeaveExit()
    {
        Vector2 outTarget = MoonHunter.Instance.RelativeToGlobal(new Vector2(0.5f, -0.5f));
        MoveTo(outTarget, leaveDuration);
    }

    public override void SetParameters(params object[] parameters)
    {
        if (parameters.Length == 0)
            return;

        DropType = (string)parameters[0];
        DropGroup = (string)parameters[1];
        DropPlace = (Vector2)parameters[2];

    }

    public void Drop()
    {
        loadSlot.RemoveChild(DropEnemy);
        MoonHunter.Instance.AddEnemyToEntities(DropEnemy);
        DropEnemy.Enabled = true;
        DropEnemy.Position = loadSlot.GlobalPosition;
        DropEnemy.Start();
    }

    public override void Start()
    {
        machine.ForceState(STATE_MOVING_TO);
    }
}
