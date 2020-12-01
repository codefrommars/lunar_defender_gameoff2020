
using System;
using Godot;


public delegate float ForceInterpolator(TimedForce force, float time);

public struct TimedForce
{
    public static float ConstantInterpolator(TimedForce force, float time)
    {
        return force.value;
    }

    public Vector2 Direction;

    private float time;
    private float value;
    private float duration;
    private ForceInterpolator interpolator;

    private TimedForce(float value, Vector2 direction, float duration, ForceInterpolator interpolator)
    {
        this.time = 0;
        this.value = value;
        Direction = direction;
        this.duration = duration;
        this.interpolator = interpolator;
    }

    public bool IsOver { get { return time >= duration; } }

    public float Update(float delta)
    {
        if (IsOver)
            return 0;

        time += delta;
        return interpolator.Invoke(this, time);
    }

    public static TimedForce Constant(float value, Vector2 direction, float duration)
    {
        return new TimedForce(value, direction, duration, ConstantInterpolator);
    }
}