using Godot;
using System;

public class ScreenManager : Node2D
{
    [Export]
    private PackedScene gameplayScreen;
    [Export]
    private PackedScene titleScreen;
    [Export]
    private PackedScene gameOverScreen;
    [Export]
    private PackedScene controlsScreen;

    public static ScreenManager Instance { get; private set; }
    private ScreenManager() { }


    public Node CurrentScene { get; set; }
    public const float FADE_DURATION = 1.0f;
    private AnimationPlayer fadeAnimationPlayer;

    public override void _Ready()
    {
        fadeAnimationPlayer = GetNode<AnimationPlayer>("FadeAnimationPlayer");
        Instance = this;
        
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        FadeIn();

        VisualServer.SetDefaultClearColor(Color.Color8(0, 0, 0, 255));
    }

    public GameplayScreen GoToGameplay(MoonHunterState gameState)
    {
        GameplayScreen screen = (GameplayScreen)gameplayScreen.Instance();
        PackedScene levelScene = GD.Load<PackedScene>(gameState.CurrentLevel.LevelFile);
        screen.CurrentLevel = (BaseLevel)levelScene.Instance();

        GotoScreen(screen);

        return screen;
    }

    public void GoToControls()
    {
        ControlsScreen screen = (ControlsScreen)controlsScreen.Instance();
        GotoScreen(screen);
    }

    public void GoToTitle()
    {
        TitleScreen screen = (TitleScreen)titleScreen.Instance();
        GotoScreen(screen);
    }

    public void GoToGameOver()
    {
        Node2D screen = (Node2D)gameOverScreen.Instance();
        GotoScreen(screen);
    }

    protected void GotoScreen(Node2D screen)
    {
        Godot.Collections.Array args = new Godot.Collections.Array();
        args.Add(screen);

        ConnectOneShotCallback(nameof(DoChangeScreen), args);
        FadeOut();
    }

    private void DoChangeScreen(string anim, Node2D nextScene)
    {
        CallDeferred(nameof(DeferredChangeScreen), nextScene);
    }

    private void DeferredChangeScreen(Node2D nextScene)
    {
        CurrentScene.Free();
        CurrentScene = nextScene;

        GetTree().Root.AddChild(CurrentScene);
        GetTree().CurrentScene = CurrentScene;
        FadeIn();
    }

    public void FadeIn(float duration = FADE_DURATION)
    {
        fadeAnimationPlayer.PlaybackSpeed = 1 / duration;
        fadeAnimationPlayer.Play("FadeIn");
    }

    private void ConnectOneShotCallback(string callbackMethod, Godot.Collections.Array args = null, ConnectFlags flags = 0)
    {
        uint uIntFlags = (uint)(ConnectFlags.Oneshot | flags);
        fadeAnimationPlayer.Connect("animation_finished", this, callbackMethod, args, uIntFlags);
    }

    public void FadeOut(float duration = FADE_DURATION)
    {
        fadeAnimationPlayer.PlaybackSpeed = 1 / duration;
        fadeAnimationPlayer.Play("FadeOut");
    }
}
