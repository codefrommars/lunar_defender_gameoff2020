using Godot;


public struct Spawner
{
    public Vector2 Position;
    public int FacingDir;
}

[Tool]
public class Stage : ReferenceRect
{
    [Export]
    public float Width { get; set; } = 10.0f;
    [Export]
    public bool LockPlayer { get; set; } = true;

    private Color Normal = new Color(0, 1, 0, 1);
    private Color Locked = new Color(1, 0, 0, 1);

    private Position2D playerSpawn;

    private Spawner[] spawners;
    private Vector2[] dropPlaces;
    private Door[] doors;
    private StageScript stageScript;
    private Node2D transitions;
    public Home Machine { get; private set; }
    // private Node2D doors;

    public override void _Ready()
    {
        if (!Engine.EditorHint)
            GetNode<Label>("NameLabel").QueueFree();

        if (HasNode("Machine"))
        {
            Machine = GetNode<Home>("Machine");
        }

        playerSpawn = GetNode<Position2D>("PlayerSpawn");

        Node2D spawnersNode = GetNode<Node2D>("Spawners");
        spawners = new Spawner[spawnersNode.GetChildCount()];
        for (int i = 0; i < spawners.Length; i++)
        {
            RayCast2D ray = spawnersNode.GetChild<RayCast2D>(i);
            spawners[i].FacingDir = (int)ray.Scale.x;
            spawners[i].Position = ray.GlobalPosition;
        }

        Node2D dropPlacesNode = GetNode<Node2D>("DropPlaces");
        dropPlaces = new Vector2[dropPlacesNode.GetChildCount()];
        for (int i = 0; i < dropPlaces.Length; i++)
        {
            Position2D dropPlace = dropPlacesNode.GetChild<Position2D>(i);
            dropPlaces[i] = dropPlace.GlobalPosition;
        }

        Node2D doorsNode = GetNode<Node2D>("Doors");
        doors = new Door[doorsNode.GetChildCount()];
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i] = doorsNode.GetChild<Door>(i);
        }



        transitions = GetNode<Node2D>("Transitions");

        // GD.Print(Name);

        if (!HasNode("ScriptSequence"))
            return;

        stageScript = GetNode<StageScript>("ScriptSequence");
        stageScript.BuildFor(this);
    }

    public void Start()
    {
        if (stageScript != null)
            stageScript.Start();
    }

    public Spawner GetSpawner(int index)
    {
        return spawners[index];
    }

    public Vector2 GetDropPlace(int index)
    {
        return dropPlaces[index];
    }

    private void ResetName()
    {
        Label nameLabel = GetNode<Label>("NameLabel");
        nameLabel.Text = Name;
    }

    public override void _Draw()
    {
        if (!Engine.EditorHint)
            return;

        Color color = Normal;
        if (LockPlayer)
            color = Locked;

        Rect2 rect = new Rect2(0f, 0f, RectSize);
        DrawRect(rect, color, false, Width, false);
        ResetName();
    }

    public string GetCurrentStageName()
    {
        return GetParent().Name;
    }

    public StageTransition GetTransition(string stageName)
    {
        if (!transitions.HasNode(stageName))
            return null;

        return transitions.GetNode<StageTransition>(stageName);
    }

    public void SetTransitionsLock(bool lockState)
    {
        for (int i = 0; i < transitions.GetChildCount(); i++)
        {
            transitions.GetChild<StageTransition>(i).Locked = lockState;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            if (!lockState)
                doors[i].Open();
            else
                doors[i].Close();
        }
    }

    // public bool HasPlayerSpawn()
    // {
    //     return HasNode("PlayerSpawn");
    // }

    public Vector2 GetPlayerSpawn()
    {
        return playerSpawn.GlobalPosition;
    }

}
