

using Godot;

public class GameplayScreen : Node2D
{
    [Signal]
    public delegate void OnStageReady();

    public TransitionCamera MainCamera { get; private set; }
    private Node2D projectilesNode;
    private Node2D stageNode;
    public Node2D Entities { get; private set; }
    public Label MessageLabel { get; private set; }
    public Node2D MessageNode { get; private set; }


    public Hunter Player { get; private set; }

    //This is set when changing the scene
    public Stage CurrentStage { get; set; }
    public BaseLevel CurrentLevel { get; set; }
    public Node Background { get; set; }

    public FramedMessage FramedMessage { get; private set; }

    private FSM.State STATE_NORMAL, STATE_TRANSITIONING;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        projectilesNode = GetNode<Node2D>("Projectiles");
        // GD.Print("Loaded GameplayScreen " + this);
        stageNode = GetNode<Node2D>("Stage");
        Entities = GetNode<Node2D>("Entities");
        MainCamera = GetNode<TransitionCamera>("MainCamera");
        MessageNode = GetNode<Node2D>("UILayer/MessageNode");
        MessageLabel = GetNode<Label>("UILayer/MessageNode/MessageLabel");
        FramedMessage = GetNode<FramedMessage>("UILayer/FramedMessage");

        MoonHunter.Instance.RegisterGameplayScreen(this);
        SetupStage(CurrentLevel, Background, true);

        if (!SoundManager.Instance.GameAudioPlayer.Playing)
        {
            SoundManager.Instance.StartGameAudioPlayer.Stop();
            SoundManager.Instance.GameAudioPlayer.Play();
        }


    }

    private BaseLevel SetupStage(BaseLevel level, Node background, bool initial = false)
    {
        stageNode.AddChild(level);

        Hunter hunter = Hunter.Instance();
        //Moved to ready
        Entities.AddChild(hunter);

        this.Player = hunter;

        //Moved to ready
        // Rect2 rect = level.GetExtentsRect();
        // SetCameraLimits(rect);
        // CurrentStage = level.GetInitialStage();
        CurrentStage = level.GetStage(MoonHunter.Instance.GameState.LastSaveStage);

        //if (CurrentStage.HasPlayerSpawn())
        hunter.GlobalPosition = CurrentStage.GetPlayerSpawn();
        // else
        //     hunter.GlobalPosition = level.PlayerStart.GlobalPosition;

        SetCameraLimits(CurrentStage.GetGlobalRect());

        MainCamera.Target = hunter;
        MainCamera.Current = true;
        MainCamera.LockTarget = CurrentStage.LockPlayer;

        // ParallaxBackground p = (ParallaxBackground)background;
        // AddChild(background);

        SetupLevelStage(!initial);

        return level;
    }



    protected void SetupLevelStage(bool runStageScript)
    {
        if (runStageScript)
            CurrentStage.Start();
        // MoonHunter.Instance.StartLevelScript(script);
    }

    public void SetCameraLimits(Rect2 rect)
    {
        MainCamera.LimitLeft = Mathf.Max(0, (int)rect.Position.x);
        MainCamera.LimitRight = (int)(rect.Position.x + rect.Size.x);
        MainCamera.LimitTop = (int)rect.Position.y;
        MainCamera.LimitBottom = (int)(rect.Position.y + rect.Size.y);
    }

    private bool slow = false;


    public void AddProjectile(Node projectile)
    {
        // if (projectilesNode != null)
        projectilesNode.AddChild(projectile);
    }

    //This is a deferred call
    private void TransitionEnded(StagePortal portal)
    {
        SetupLevelStage(true);
        FreeEnemiesInGroup(MoonHunter.Constants.OTHER_ENEMIES_GROUP);
    }

    private void TriggerTransition(StageTransition transition)
    {
        string nextStageName = transition.GetNextStageName();
        Stage nextStage = CurrentLevel.GetStage(nextStageName);

        // StageTransition mirrorTransition = transition.GetMirror(nextStage);

        MainCamera.ExecutePortalTransition(nextStage, transition);

        CurrentStage = nextStage;
    }

    public int GetEnemiesInGroup(string group)
    {
        if (GetTree() == null)
            return 0;

        return GetTree().GetNodesInGroup(group).Count;
    }

    public void FreeEnemiesInGroup(string group)
    {
        // Godot.Collections.Array array = GetTree().GetNodesInGroup(group);
        // GD.Print("Freeing Enemies in group: " + group);
        // foreach (Node node in array)
        //     node.QueueFree();

        GetTree().CallGroup(group, "queue_free");
    }

    public void ShowMessage(string message, Vector2 position)
    {
        MessageLabel.Text = message;
        MessageLabel.Visible = true;
        MessageNode.Position = MainCamera.GetUIPosition(position);
    }

    public void HideMessage()
    {
        MessageLabel.Visible = false;
    }
}
