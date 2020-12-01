

using Godot;
using System;

public struct Transition
{
    public Vector2 initialPosition, targetPosition;
    public Rect2 initialLimits, targetLimits;
    public Stage nextStage;
    public StageTransition portal, mirrorPortal;

    public float duration;
    public float time;

    public float Update(float delta)
    {
        time += delta;
        return Math.Min(1, time / duration);
    }

    public Vector2 PositionAt(float ratio)
    {
        return initialPosition.LinearInterpolate(targetPosition, ratio);
    }

    public Rect2 LimitsAt(float ratio)
    {
        Rect2 rect = new Rect2();
        rect.Position = initialLimits.Position.LinearInterpolate(targetLimits.Position, ratio);
        rect.Size = initialLimits.Size.LinearInterpolate(targetLimits.Size, ratio);
        return rect;
    }

    public bool IsOver()
    {
        return time >= duration;
    }
}

