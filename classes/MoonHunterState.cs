
using System;
using Godot;

public class MoonHunterState
{

    public enum Powerup
    {
        Blocks,
        Mines,
        Beam
    };

    public LevelManager.LevelInfo CurrentLevel { get; set; }
    public string LastSaveStage { get; set; } = "Start";

    public static long Shots;
    public static long Mines;
    public static long LaserBeams;
    public static long Boxes;

    public static long EnemiesDestroyed;
    public static long DeathsRestarts;
    public static long LootCollected;
    public static ulong StartDate = OS.GetUnixTime();
    private bool[] unlockedPowerUps = { false, false, false };

    public MoonHunterState()
    {
        CurrentLevel = LevelManager.Levels.LevelTest;
    }

    public bool OnPlayerDied()
    {
        DeathsRestarts++;
        return true;
    }

    public void UnlockPowerup(Powerup powerup)
    {
        unlockedPowerUps[(int)powerup] = true;
    }

    public bool IsUnlocked(Powerup powerup)
    {
        return unlockedPowerUps[(int)powerup];
    }

    public string GetMissionCompleted()
    {
        ulong now = OS.GetUnixTime();

        ulong elapsed = now - StartDate;
        int minutes = (int)(elapsed / 60);
        int seconds = (int)(elapsed % 60);

        string text = "Time taken: " + minutes + ":" + seconds + "\n";
        text += "Deaths: " + DeathsRestarts + "\n";
        text += "Shots: " + Shots + "\n";

        if (DeathsRestarts == 0)
            text += "Rate S soldier.";
        else if (DeathsRestarts < 3)
            text += "Rate A soldier.";
        else
            text += "Average Soldier.";

        return text;
    }
}