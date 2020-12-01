using Godot;
using System;

public class TitleScreen : Node2D
{

    Button startButton;

    public override void _Ready()
    {
        startButton = GetNode<Button>("StartButton");
        startButton.GrabFocus();
        SoundManager.Instance.StartGameAudioPlayer.Play();
        // OnStartPressed();
    }

    public void OnStartPressed()
    {
        ScreenManager.Instance.GoToControls();
        startButton.Disabled = true;

    }
}
