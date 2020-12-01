
using Godot;

public class LevelManager
{
    public class LevelInfo
    {
        public string LevelFile { get; set; }
    }

    private BaseLevel level;


    public LevelManager()
    {
    }

    public void LoadLevel(LevelInfo levelInfo)
    {
        level = GD.Load<BaseLevel>(levelInfo.LevelFile);
    }
}