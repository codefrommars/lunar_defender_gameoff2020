
using Godot;

public class LevelManager
{
    public class LevelInfo
    {
        public string LevelFile { get; set; }

        // public string Directory { get; set; }
        // public string StartingStage { get; set; }
        // public string Background { get; set; }
    }

    public static class Levels
    {
        public static LevelInfo LevelTest = new LevelInfo
        {
            LevelFile = "res://levels/moon-fortress/MoonFortress.tscn"
            // LevelFile = "res://levels/level-test/LevelTest.tscn"
            // Directory = "res://levels/level-test/",
            // StartingStage = "LevelTest00",
            // Background = "LevelTestBackground"
        };
        public static LevelInfo MoonBase = new LevelInfo
        {
            LevelFile = "res://levels/moon-base/MoonBase.tscn"
            // Directory = "res://levels/moon-base/",
            // StartingStage = "MoonBase01",
            // Background = "MoonBaseBackground"

        };
    }

    // private LevelRuntime level;
    private BaseLevel level;


    public LevelManager()
    {
        // level = new LevelRuntime();
    }

    public void LoadLevel(LevelInfo levelInfo)
    {
        // if (level.currentLevel == levelInfo)
        //     return;

        // level = new LevelRuntime();
        // level.LoadLevel(levelInfo);
        level = GD.Load<BaseLevel>(levelInfo.LevelFile);
    }

    // public BaseStage GetStage(string stageKey)
    // {
    //     return level.GetStage(stageKey);
    // }

    // public Node GetBackground()
    // {
    //     return level.GetBackground();
    // }

    // public bool IsLoaded(string stageName)
    // {
    //     return level.IsLoaded(stageName);
    // }

    // public void FlushCache()
    // {
    //     level.FlushCache();
    // }
}