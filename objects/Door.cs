using Godot;
using System;

public class Door : Node2D
{
    private AnimationPlayer animationPlayer;
    private StaticBody2D body;
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        body = GetNode<StaticBody2D>("StaticBody2D");
        Visible = false;
    }

    public void Open()
    {
        SoundManager.Instance.DoorOpenAudioPlayer.Play();
        animationPlayer.Play("Open");
        body.CollisionLayer = 0;
    }

    public void Close()
    {
        SoundManager.Instance.DoorCloseAudioPlayer.Play();
        Visible = true;
    }
}
