

using Godot;
using System;
using System.Collections.Generic;

public class MoonMovement
{
    // private Vector2 jumpVelocity;
    public Vector2 inputVelocity;
    // private float damp;
    private static Vector2 UP = new Vector2(0, -1);
    public float MaxTotalSpeed { get; set; }

    // private List<TimedForce> externalForces;
    //sim movement

    public float Gravity { get; set; }
    public float GravityScale { get; set; } = 1;
    public float MaxJumpSpeed { get; private set; }
    public float MinJumpSpeed { get; private set; }
    public Vector2 GravityDirection { get; set; } = new Vector2(0, 1);

    public float MaxGravitySpeed { get; set; }
    public bool IsGrounded { get; set; }
    // public float Friction { set { damp = 1 - value; } }

    public float AirDamp { get; set; } = 0;
    public float FloorDamp { get; set; } = 0;
    public bool UseHorizontalDamp { get; set; } = false;

    //input movement
    public float InputHorizonalSpeed { get; set; }
    public float InputVerticalSpeed { get; set; }
    public bool IsFacingRight { get; private set; }
    public int FacingDirection { get; set; } = 1;


    private int horizontal;
    public int Horizontal
    {
        get { return horizontal; }

        set
        {
            if (horizontal == value)
                return;

            horizontal = value;

            if (value == 0)
                return;
            IsFacingRight = value > 0;
            FacingDirection = IsFacingRight ? 1 : -1;
        }
    }

    public void SetupJumpPhysics(float maxJumpHeightPixels, float minJumpHeightPixels, float jumpToPeakDurationSeconds)
    {
        Gravity = 2.0f * maxJumpHeightPixels / (jumpToPeakDurationSeconds * jumpToPeakDurationSeconds);
        MaxJumpSpeed = -(float)Math.Sqrt(2 * Gravity * maxJumpHeightPixels);
        MinJumpSpeed = -(float)Math.Sqrt(2 * Gravity * minJumpHeightPixels);
    }

    public void ApplyMotion(float x, float y = 0)
    {
        if (UseHorizontalDamp)
            inputVelocity.x += x * InputHorizonalSpeed;
        else
            inputVelocity.x = x * InputHorizonalSpeed;

        Horizontal = (int)x;
    }

    public void ApplyNonFacingMotion(float x, float y = 0)
    {
        if (UseHorizontalDamp)
            inputVelocity.x += x * InputHorizonalSpeed;
        else
            inputVelocity.x = x * InputHorizonalSpeed;
    }

    public void Move(KinematicBody2D character, float delta)
    {
        inputVelocity.y += GravityScale * Gravity * delta;

        //Move in X
        inputVelocity = character.MoveAndSlide(inputVelocity, UP, true, 4, 0.785398f, false);

        if (UseHorizontalDamp)
        {
            float damp = AirDamp;

            if (character.IsOnFloor())
                damp = FloorDamp;

            inputVelocity.x *= damp;
        }
        // inputVelocity.x *= 0.2f;
    }

    // public void Recoil(Vector2 direction, float speed)
    // {
    //     inputVelocity += direction * speed;
    // }

    public void Jump()
    {
        inputVelocity.y = MaxJumpSpeed;
    }

    public void CapJumpSpeed()
    {
        if (inputVelocity.y < MinJumpSpeed)
            inputVelocity.y = MinJumpSpeed;
    }

    public bool IsFalling()
    {
        return inputVelocity.y > 0;
    }

    public void ResetVerticalSpeed()
    {
        inputVelocity.y = 0;
    }

}