
using Godot;
using System;

public class TransitionCamera : Camera2D
{

    public Node2D Target { get; set; }
    public bool LockTarget { get; set; }

    private FSM.State STATE_FOLLOWING, STATE_TRANSITIONING;
    private FSM behavior;
    private Transition transition;

    public TransitionCamera()
    {
        STATE_FOLLOWING = new FSM.State(OnFollowing) { Name = "Following" };
        STATE_TRANSITIONING = new FSM.State(OnTransitioning, OnTransitionStart, OnTransitionCompleted) { Name = "Transitioning" };

        behavior = new FSM(STATE_FOLLOWING);
    }

    public override void _Process(float delta)
    {
        behavior.Update(delta);
    }


    public FSM.State OnFollowing(float delta)
    {
        if (LockTarget)
        {
            Rect2 limits = GetLimits();
            if (!limits.HasPoint(Target.Position))
            {
                Target.Position = ClampToLimits(Target.Position);
            }
        }
        if (Target != null)
            Position = Target.Position;



        return STATE_FOLLOWING;
    }

    private Vector2 ClampToLimits(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, LimitLeft, LimitRight);
        position.y = Mathf.Clamp(position.y, LimitTop, LimitBottom);

        return position;
    }

    public Rect2 GetVisibleRect()
    {
        Transform2D transform = GetCanvasTransform();
        return new Rect2(-transform.origin / transform.Scale, GetViewportRect().Size / transform.Scale);
    }

    public Vector2 GetUIPosition(Vector2 relative)
    {
        Rect2 visibleRect = GetVisibleRect();
        return visibleRect.Size * relative;
    }

    public Vector2 RelativeToGlobal(Vector2 relative)
    {
        Rect2 visibleRect = GetVisibleRect();
        return visibleRect.Position + visibleRect.Size * relative;
    }

    private FSM.State OnTransitioning(float delta)
    {
        float weight = transition.Update(delta);
        SetLimits(transition.LimitsAt(weight));
        Position = Target.Position = transition.PositionAt(weight);

        if (transition.IsOver())
        {
            return STATE_FOLLOWING;
        }

        return STATE_TRANSITIONING;
    }

    private void OnTransitionStart()
    {
        transition.time = 0f;
    }
    private void OnTransitionCompleted()
    {
        MoonHunter.Instance.TransitionEnded(transition.portal);

        if (transition.mirrorPortal != null)
            transition.mirrorPortal.Active = true;

        Stage newStage = transition.nextStage;
        SetLimits(newStage.GetGlobalRect());

        int vertical = (int)transition.portal.Direction.y;
        if (vertical == 1)
        {
            MoonHunter.Instance.Player.Movement.ResetVerticalSpeed();
        }
        else if (vertical == -1)
        {
            MoonHunter.Instance.Player.ForceJump();
        }
        MoonHunter.Instance.Player.Enabled = true;

        LockTarget = newStage.LockPlayer;
    }

    public void SetLimits(Rect2 rect)
    {
        LimitLeft = (int)rect.Position.x;
        LimitRight = (int)(rect.Position.x + rect.Size.x);
        LimitTop = (int)rect.Position.y;
        LimitBottom = (int)(rect.Position.y + rect.Size.y);
    }

    private Rect2 GetLimits()
    {
        Rect2 rect = new Rect2();
        rect.Position = new Vector2(LimitLeft, LimitTop);
        rect.Size = new Vector2(LimitRight - LimitLeft, LimitBottom - LimitTop);
        return rect;
    }

    public void ExecutePortalTransition(Stage nextStage, StageTransition portal)
    {
        transition.portal = portal;
        transition.mirrorPortal = portal.GetMirror(nextStage);
        transition.nextStage = nextStage;

        float transitionDuration = MoonHunter.Constants.PORTAL_TRANSITION_DURATION;
        Hunter player = MoonHunter.Instance.Player;
        player.Enabled = false;

        Vector2 to = portal.GetTransitionPosition(player);
        StartTransition(to, portal.Direction, transitionDuration);
    }

    public void StartTransition(Vector2 targetPosition, Vector2 direction, float duration)
    {
        transition.portal.Active = false;
        if (transition.mirrorPortal != null)
            transition.mirrorPortal.Active = false;

        transition.initialPosition = Position;
        transition.targetPosition = targetPosition;

        Rect2 currentView = GetVisibleRect();
        transition.initialLimits = transition.targetLimits = currentView;
        transition.targetLimits.Position += direction * currentView.Size;
        transition.duration = duration;

        behavior.ForceState(STATE_TRANSITIONING);
    }

}
