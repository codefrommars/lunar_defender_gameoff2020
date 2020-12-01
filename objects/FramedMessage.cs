using Godot;
using System;

public class FramedMessage : Node2D
{
    private Label title, content;
    private Button accept;


    public override void _Ready()
    {
        title = GetNode<Label>("Title");
        content = GetNode<Label>("Content");
        accept = GetNode<Button>("AcceptButton");
        accept.GrabFocus();
    }

    public void OnAcceptClick()
    {
        Visible = false;
        // GD.Print("CLICKED !!!");
        GetTree().Paused = false;
    }

    public void ShowMessage(string content, string title = "Message")
    {
        this.title.Text = title;
        this.content.Text = content;

        Visible = true;
        GetTree().Paused = true;
        accept.GrabFocus();
    }


}
