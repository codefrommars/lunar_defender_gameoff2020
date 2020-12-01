using Godot;
using System;

public class GameOverScreen : Node2D
{
    Button continueButton;

    public override void _Ready()
    {
        continueButton = GetNode<Button>("ContinueButton");
    }

    public void OnStartPressed()
    {
        MoonHunter.Instance.StartGame();
        continueButton.Disabled = true;

    }
}
