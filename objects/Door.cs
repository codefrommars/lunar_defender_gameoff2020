using Godot;
using System;

public class Door : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private AnimationPlayer animationPlayer;
    private StaticBody2D body;
    // private Sprite up, down;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // up = GetNode<Sprite>("door_up ");
        // down = GetNode<Sprite>("door_down ");
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
        //animationPlayer.PlayBackwards("Open");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
