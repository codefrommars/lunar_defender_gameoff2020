using Godot;
using System;

public class ControlsScreen : Node2D
{

    Button startButton;

    public override void _Ready()
    {
        startButton = GetNode<Button>("StartButton");
        startButton.GrabFocus();
    }

    public void OnStartPressed()
    {
        MoonHunter.Instance.StartGame();
        startButton.Disabled = true;

    }
}