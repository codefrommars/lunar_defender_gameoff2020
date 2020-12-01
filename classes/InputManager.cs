

using Godot;
using System;

public class InputManager
{
    public static Vector2 Direction
    {
        get
        {
            Vector2 direction = new Vector2();

            int analogX = Math.Sign(Input.GetActionStrength("analog_right") - Input.GetActionStrength("analog_left"));
            int analogY = Math.Sign(Input.GetActionStrength("analog_down") - Input.GetActionStrength("analog_up"));
            int digitalX = Convert.ToInt32(Input.IsActionPressed("ui_right")) - Convert.ToInt32(Input.IsActionPressed("ui_left"));
            int digitalY = Convert.ToInt32(Input.IsActionPressed("ui_down")) - Convert.ToInt32(Input.IsActionPressed("ui_up"));
            direction.x = analogX + digitalX;
            direction.y = analogY + digitalY;
            return direction;
        }
    }

    public static bool JumpPressed
    {
        get
        {
            return Input.IsActionPressed("ui_jump");
        }
    }

    public static bool DashPressed
    {
        get
        {
            return Input.IsActionJustPressed("ui_dash");
        }
    }

    public static bool ShootPressed
    {
        get
        {
            return Input.IsActionPressed("ui_shoot");
        }
    }

}