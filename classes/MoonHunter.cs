
using System;
using Godot;


public class MoonHunter
{
    public static class Constants
    {
        public const int TILE_SIZE = 60;

        public const float AIR_FRICTION = 0.01f;
        public const float FLOOR_FRICTION = AIR_FRICTION;//0.95f;
        public const float PORTAL_TRANSITION_DURATION = 1.0f;
        public const float PLAYER_HORIZONTAL_SPEED = 7 * TILE_SIZE;
        public const float PLAYER_MAX_TOTAL_SPEED = 15 * TILE_SIZE;
        public const float PLAYER_MAX_GRAVITY_SPEED = 10 * TILE_SIZE;

        public const float PLAYER_JUMP_HEIGHT = 4.25f * TILE_SIZE;
        public const float PLAYER_MIN_JUMP_HEIGHT = 1.8f * TILE_SIZE;
        public const float PLAYER_JUMP_DURATION = 0.4f;

        public const float PLAYER_DASH_LENGTH = 5.2f * TILE_SIZE;
        public const float PLAYER_DASH_DURATION = 0.2f;
        public const float PLAYER_DASH_SPEED = PLAYER_DASH_LENGTH / PLAYER_DASH_DURATION;

        public const float PLAYER_HURT_SPEED = 50f;


        public const string TARGET_ENEMIES_GROUP = "Enemies";
        public const string OTHER_ENEMIES_GROUP = "Other";

        public const int LASER_BEAM_DAMAGE = 50;
        public const int LASER_BEAM_COST = 40;

        public const int PLAYER_MAX_ENERGY = 1000;
        public const int MACHINE_MAX_ENERGY = 100;
        public const int MAX_MINES_IN_GAME = 5;
        public const int MIN_HP_FOR_PLACING = 50;
        public const int MINE_COST = 50;
        public const int BOX_COST = 20;

        public const int WALKER_ENERGY = 50;
        public const int SAW_ENERGY = 60;
        public const int SAW_TROOPER_ENERGY = 80;

        public const int TOUCH_DAMAGE = 25;

        public const int DEFAULT_BULLET_ENEMY_DAMAGE = 60;
        public const int DEFAULT_LOOT = 20;
        public const int INITIAL_PLAYER_HP = 500;
    }

    public static class PhysicsLayers
    {
        public const uint Player = 1;
        public const uint Level = 2;
        public const uint PlayerHitbox = 4;
        public const uint Enemy = 8;
        public const uint EnemyHitbox = 16;
        public const uint PlayerBullet = 32;
        public const uint EnemyBullet = 32;

    }

    public Random Rand { get; private set; }

    private MoonHunter()
    {
        LevelManager = new LevelManager();
        WHITE_MATERIAL = GD.Load<Material>("res://assets/materials/White.tres");
        Rand = new Random(444777);
    }

    public static MoonHunter Instance { get; } = new MoonHunter();

    public Hunter Player { get { return gameplayScreen.Player; } }
    public LevelManager LevelManager { get; }
    public MoonHunterState GameState { get; private set; }
    public TransitionCamera Camera { get { return gameplayScreen.MainCamera; } }


    public Material WHITE_MATERIAL { get; private set; }
    private GameplayScreen gameplayScreen;

    public void StartGame()
    {
        GameState = new MoonHunterState();
        GoToLastSave();
    }

    public void RegisterGameplayScreen(GameplayScreen screen)
    {
        gameplayScreen = screen;
    }

    public void GoToLastSave()
    {

        placedMines = 0;
        ScreenManager.Instance.GoToGameplay(GameState);
    }

    public void PlayerDied()
    {
        if (!GameState.OnPlayerDied())
        {
            GameOver();
            return;
        }

        GoToLastSave();

    }

    public void GameOver()
    {
        ScreenManager.Instance.GoToGameOver();
    }

    public LevelScriptRunner StartLevelScript(ScriptAction script)
    {
        LevelScriptRunner runner = InstanceResource<LevelScriptRunner>("res://levels/LevelScriptRunner.tscn");
        runner.Script = script;
        gameplayScreen.AddChild(runner);
        return runner;
    }

    public T InstanceResource<T>(string resource) where T : Node
    {
        PackedScene scene = GD.Load<PackedScene>(resource);
        Node inst = scene.Instance();
        return (T)inst;
    }

    public Box AddBox(Vector2 globalPosition)
    {
        Box box = InstanceResource<Box>("res://objects/Box.tscn");
        box.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(box);

        SoundManager.Instance.EnergyBoxAudioPlayer.Play();
        MoonHunterState.Boxes += 1;
        return box;
    }

    public Grenade AddGrenade(Vector2 globalPosition, Vector2 direction)
    {
        Grenade grenade = InstanceResource<Grenade>("res://objects/Grenade.tscn");
        grenade.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(grenade);
        grenade.ApplyImpulse(new Vector2(20, 20), direction * 800);
        MoonHunterState.Mines += 1;
        return grenade;
    }

    private int placedMines = 0;
    public Mine AddMine(Vector2 globalPosition)
    {
        Mine mine = InstanceResource<Mine>("res://objects/Mine.tscn");
        mine.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(mine);
        SoundManager.Instance.ArmMineAudioPlayer.Play();
        MoonHunterState.Mines += 1;
        placedMines++;

        return mine;
    }

    public void MineExploded()
    {
        placedMines--;
    }

    public int CountPlacedMinesInStage()
    {
        return placedMines;
    }

    public T NewProjectile<T>(PackedScene scene) where T : Node
    {
        T p = (T)scene.Instance();
        gameplayScreen.AddProjectile(p);
        return p;
    }

    public void AddBulletExplosion(Vector2 globalPosition, float rotation)
    {
        BulletExplosion explosion = InstanceResource<BulletExplosion>("res://objects/BulletExplosion.tscn");
        explosion.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(explosion);
        explosion.Start(rotation);
    }

    public void AddBulletHit(Vector2 globalPosition)
    {
        HitImpact impact = InstanceResource<HitImpact>("res://objects/HitImpact.tscn");
        impact.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(impact);
        impact.Start();
    }

    public void AddLoot(Vector2 globalPosition, int total, int n)
    {
        int ammount = (int)Math.Ceiling((double)total / n);
        Random rand = new Random();
        for (int i = 0; i < n; i++)
        {
            InstanceLoot(globalPosition, rand, ammount);
        }
    }



    public Loot InstanceLoot(Vector2 globalPosition, Random rand, int ammount)
    {
        Loot loot = InstanceResource<Loot>("res://objects/Loot.tscn");
        loot.GlobalPosition = globalPosition;
        loot.Ammount = ammount;

        gameplayScreen.CallDeferred("AddProjectile", loot);

        loot.Inertia = (float)rand.NextDouble() * 500;

        float fx = (float)(100 * (1 - 2 * rand.NextDouble()));
        float fy = 400 + (float)(100 * (1 - 2 * rand.NextDouble()));
        loot.ApplyImpulse(new Vector2(12, 12), new Vector2(fx, -fy));

        return loot;
    }

    public void AddExplosion(Vector2 globalPosition)
    {
        Explosion impact = InstanceResource<Explosion>("res://objects/Explosion.tscn");
        impact.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(impact);
        impact.Start();
    }

    public LaserBeam AddLaserBeam(Vector2 globalPosition, float globalRotation)
    {
        LaserBeam laserBeam = InstanceResource<LaserBeam>("res://objects/LaserBeam.tscn");
        laserBeam.GlobalPosition = globalPosition;
        laserBeam.GlobalRotation = globalRotation;
        gameplayScreen.AddProjectile(laserBeam);
        MoonHunterState.LaserBeams += 1;

        return laserBeam;
    }

    public void AddMovementDust(Vector2 globalPosition)
    {
        Dust dustP = InstanceResource<Dust>("res://objects/Dust.tscn");
        dustP.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(dustP);
        Dust dustN = InstanceResource<Dust>("res://objects/Dust.tscn");
        dustN.GlobalPosition = globalPosition;
        gameplayScreen.AddProjectile(dustN);
        dustN.Scale = new Vector2(-1, 1);
        dustP.Start();
        dustN.Start();
    }

    public void TriggerTransition(StageTransition transition)
    {
        gameplayScreen.CallDeferred("TriggerTransition", transition);
    }

    public void TransitionEnded(StageTransition transition)
    {
        gameplayScreen.CallDeferred("TransitionEnded", transition);
        placedMines = 0;
    }

    public void RemoveEnemyFromEntities(BaseEnemy enemy)
    {
        gameplayScreen.Entities.RemoveChild(enemy);
    }

    public void AddEnemyToEntities(BaseEnemy enemy)
    {
        gameplayScreen.Entities.AddChild(enemy);
    }

    public Spawner GetSpawner(int index)
    {
        return gameplayScreen.CurrentStage.GetSpawner(index);
    }

    public Vector2 GetDropPlace(int dropIndex)
    {
        return gameplayScreen.CurrentStage.GetDropPlace(dropIndex);
    }

    public void FreeEnemiesInGroup(string group)
    {
        gameplayScreen.FreeEnemiesInGroup(group);
    }

    public BaseEnemy SimpleSpawnEnemy(string enemyType, string group = "Default", params object[] parameters)
    {
        BaseEnemy enemy = InstanceResource<BaseEnemy>(enemyType);
        enemy.SetParameters(parameters);
        enemy.AddToGroup(group);
        gameplayScreen.Entities.AddChild(enemy);

        return enemy;
    }

    public BaseEnemy SpawnEnemyAtSpawner(int spawnerIndex, string enemyType, string group, params object[] parameters)
    {
        Spawner spawner = MoonHunter.Instance.GetSpawner(spawnerIndex);
        BaseEnemy enemy = MoonHunter.Instance.SimpleSpawnEnemy(enemyType, group, parameters);
        enemy.GlobalPosition = spawner.Position;
        enemy.SetFacingDirection(spawner.FacingDir);
        enemy.Start();
        return enemy;
    }

    public DroneDrop DropEnemy(int dropIndex, string enemyType, string group, params object[] parameters)
    {
        Vector2 dropPlace = GetDropPlace(dropIndex);

        DroneDrop dropDrone = (DroneDrop)MoonHunter.Instance.SimpleSpawnEnemy(StageScript.DRONE_DROP, MoonHunter.Constants.OTHER_ENEMIES_GROUP,
                                enemyType, group, dropPlace, parameters);
        dropDrone.GlobalPosition = dropPlace + new Vector2(2000, 0);
        dropDrone.Start();

        return dropDrone;
    }

    public int GetTargetEnemiesCount()
    {
        return gameplayScreen.GetEnemiesInGroup(Constants.TARGET_ENEMIES_GROUP);
    }

    public void ShowMessage(string message, Vector2 position)
    {
        gameplayScreen.ShowMessage(message, position);
    }

    public void HideMessage()
    {
        gameplayScreen.HideMessage();
    }

    public Vector2 RelativeToGlobal(Vector2 percentage)
    {
        return gameplayScreen.MainCamera.RelativeToGlobal(percentage);
    }

    public int GetRandomSign()
    {
        if (Rand.NextDouble() < 0.5f)
            return -1;

        return 1;
    }

    public int GetRandomInt(int max = 1)
    {
        return (int)Math.Round(Rand.NextDouble() * max);
    }

    public bool GetRandomBool()
    {
        return Rand.NextDouble() < 0.5;
    }

    public float GetRandom(float min, float max)
    {
        return (float)(Rand.NextDouble() * (max - min)) + min;
    }
    public float GetRandom(float max)
    {
        return (float)(Rand.NextDouble() * max);
    }

    public Vector2 GetRandom(Rect2 rect)
    {
        return GetRandom(
            rect.Position.x, rect.Position.x + rect.Size.x,
            rect.Position.y, rect.Position.y + rect.Size.y
        );

    }
    public Vector2 GetRandom(float xMin, float xMax, float yMin, float yMax)
    {
        float dx = xMax - xMin;
        float dy = yMax - yMin;

        return new Vector2(
            (float)(xMin + Rand.NextDouble() * dx),
            (float)(yMin + Rand.NextDouble() * dy)
        );
    }

    public void DoCheckPoint(Stage stage)
    {
        MoonHunter.Instance.GameState.LastSaveStage = stage.Name;
        GD.Print("Checkpoint: " + stage.Name);
    }

    public void ShowFramedMessage(string content, string title = "Message")
    {
        gameplayScreen.FramedMessage.ShowMessage(content, title);
    }

    public void UnlockPowerup(MoonHunterState.Powerup powerup)
    {
        GameState.UnlockPowerup(powerup);
    }

    public Home GetCurrentMachine()
    {
        return gameplayScreen.CurrentStage.Machine;
    }
}
