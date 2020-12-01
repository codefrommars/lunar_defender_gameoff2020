
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

        // currentLevel = MoonHunter.Levels.LevelTest;
        // LoadLevel(currentLevel);
        // StartStage(currentLevel.StartingStage);
    }

    // public void LoadLevel(LevelManager.LevelInfo levelInfo)
    // {
    //     currentLevelStages = new Dictionary<string, PackedScene>();

    //     Directory dir = new Directory();
    //     var err = dir.Open(levelInfo.Directory);

    //     if (err != Error.Ok)
    //     {
    //         GD.PrintErr("Can't load the level at folder: " + levelInfo.Directory);
    //         return;
    //     }

    //     dir.ListDirBegin(true, true);
    //     string file = dir.GetNext();
    //     while (file != "")
    //     {
    //         if (!dir.CurrentIsDir())
    //         {
    //             string stageName = file.BaseName();
    //             string fullpath = dir.GetCurrentDir() + "/" + file;
    //             PackedScene stagePackedScene = GD.Load<PackedScene>(fullpath);
    //             currentLevelStages[stageName] = stagePackedScene;
    //             GD.Print("Loaded: " + fullpath + " as " + file.BaseName());
    //         }

    //         file = dir.GetNext();
    //     }
    //     currentLevel = levelInfo;
    // }


    // public BaseStage GetStage(string stageKey)
    // {
    //     if (loadedStages.ContainsKey(stageKey))
    //         return loadedStages[stageKey];

    //     BaseStage cached = (BaseStage)currentLevelStages[stageKey].Instance();
    //     loadedStages[stageKey] = cached;

    //     return cached;
    // }

    // public Node GetBackground()
    // {
    //     if (background == null)
    //         background = currentLevelStages[currentLevel.Background].Instance();

    //     return background;
    // }

    // public bool IsLoaded(string stageName)
    // {
    //     return loadedStages.ContainsKey(stageName);
    // }

    // public void FlushCache()
    // {
    //     foreach (BaseStage stage in loadedStages.Values)
    //         if (Object.IsInstanceValid(stage))
    //             stage.Free();

    //     loadedStages.Clear();
    //     background = null;
    // }
}