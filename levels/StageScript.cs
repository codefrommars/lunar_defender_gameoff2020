using Godot;
using System;

public class StageScript : Node
{
    public const string WALKER = "res://enemies/walker/Walker.tscn";
    public const string SAW = "res://enemies/saw/Saw.tscn";
    public const string SAW_TROOPER = "res://enemies/saw/SawTrooper.tscn";
    public const string CAMPER = "res://enemies/camper/Camper.tscn";
    public const string DRONE_A = "res://enemies/drone-a/DroneA.tscn";
    public const string DRONE_DROP = "res://enemies/drone-a/DroneDrop.tscn";

    protected ScriptAction Script { get; private set; }

    public override void _Ready()
    {
        SetPhysicsProcess(false);
    }

    public void BuildFor(Stage stage)
    {
        Script = DoBuildFor(stage);
    }

    protected virtual ScriptAction DoBuildFor(Stage stage)
    {
        return new ScriptAction();
    }

    public void Start()
    {
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(float delta)
    {

        if (Script.Act(delta))
        {
            OnCompleted();
            return;
        }
    }

    protected void OnCompleted()
    {
        SetPhysicsProcess(false);
    }
}
