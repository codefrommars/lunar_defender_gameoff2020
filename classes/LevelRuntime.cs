
using Godot;
using System.Collections.Generic;

public class LevelRuntime
{
    public LevelManager.LevelInfo currentLevel;

    private Dictionary<string, PackedScene> currentLevelStages;
    private Dictionary<string, BaseStage> loadedStages;
    private BaseStage currentStage;
    private Node background;

    public LevelRuntime()
    {
        loadedStages = new Dictionary<string, BaseStage>();
    }

}