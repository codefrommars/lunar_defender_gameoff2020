
using Godot;


public class SoundManager : Godot.Node
{
    public static SoundManager Instance { get; private set; }

    public AudioStreamPlayer DoorCloseAudioPlayer { get; private set; }
    public AudioStreamPlayer JumpAudioPlayer { get; private set; }
    public AudioStreamPlayer DashAudioPlayer { get; private set; }
    public AudioStreamPlayer ArmMineAudioPlayer { get; private set; }
    public AudioStreamPlayer DoorOpenAudioPlayer { get; private set; }
    public AudioStreamPlayer DroneAudioPlayer { get; private set; }
    public AudioStreamPlayer EnemyShootAudioPlayer { get; private set; }
    public AudioStreamPlayer EnergyBoxAudioPlayer { get; private set; }
    public AudioStreamPlayer EnergyCoinAudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion0AudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion1AudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion2AudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion3AudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion4AudioPlayer { get; private set; }
    public AudioStreamPlayer Explosion5AudioPlayer { get; private set; }
    public AudioStreamPlayer FallGroundAudioPlayer { get; private set; }
    public AudioStreamPlayer FootStepAudioPlayer { get; private set; }
    public AudioStreamPlayer RunAudioPlayer { get; private set; }
    public AudioStreamPlayer HitEnemiesAudioPlayer { get; private set; }
    public AudioStreamPlayer HitLevelAudioPlayer { get; private set; }
    public AudioStreamPlayer HurtAudioPlayer { get; private set; }
    public AudioStreamPlayer LaserBeamAudioPlayer { get; private set; }
    public AudioStreamPlayer LowEnergyAudioPlayer { get; private set; }
    public AudioStreamPlayer SawAudioPlayer { get; private set; }
    public AudioStreamPlayer ShotAudioPlayer { get; private set; }
    public AudioStreamPlayer ChargeLaserAudioPlayer { get; private set; } //Cooldown
    public AudioStreamPlayer ChargeMineAudioPlayer { get; private set; }
    public AudioStreamPlayer StartGameAudioPlayer { get; private set; }
    public AudioStreamPlayer CancelAudioPlayer { get; private set; }

    //Music
    public AudioStreamPlayer GameAudioPlayer { get; private set; }
    public AudioStreamPlayer BoosRoomAudioPlayer { get; private set; }
    public AudioStreamPlayer BossFightAudioPlayer { get; private set; }


    private AudioStreamPlayer[] enemyExplosions;
    public override void _Ready()
    {
        Instance = this;

        DoorCloseAudioPlayer = GetNode<AudioStreamPlayer>("DoorCloseAudioPlayer");
        JumpAudioPlayer = GetNode<AudioStreamPlayer>("JumpAudioPlayer");
        DashAudioPlayer = GetNode<AudioStreamPlayer>("DashAudioPlayer");
        ArmMineAudioPlayer = GetNode<AudioStreamPlayer>("ArmMineAudioPlayer");
        DoorOpenAudioPlayer = GetNode<AudioStreamPlayer>("DoorOpenAudioPlayer");
        DroneAudioPlayer = GetNode<AudioStreamPlayer>("DroneAudioPlayer");
        EnemyShootAudioPlayer = GetNode<AudioStreamPlayer>("EnemyShootAudioPlayer");
        EnergyBoxAudioPlayer = GetNode<AudioStreamPlayer>("EnergyBoxAudioPlayer");
        EnergyCoinAudioPlayer = GetNode<AudioStreamPlayer>("EnergyCoinAudioPlayer");
        Explosion0AudioPlayer = GetNode<AudioStreamPlayer>("Explosion0AudioPlayer");
        Explosion1AudioPlayer = GetNode<AudioStreamPlayer>("Explosion1AudioPlayer");
        Explosion2AudioPlayer = GetNode<AudioStreamPlayer>("Explosion2AudioPlayer");
        Explosion3AudioPlayer = GetNode<AudioStreamPlayer>("Explosion3AudioPlayer");
        Explosion4AudioPlayer = GetNode<AudioStreamPlayer>("Explosion4AudioPlayer");
        Explosion5AudioPlayer = GetNode<AudioStreamPlayer>("Explosion5AudioPlayer");
        FallGroundAudioPlayer = GetNode<AudioStreamPlayer>("FallGroundAudioPlayer");
        FootStepAudioPlayer = GetNode<AudioStreamPlayer>("FootStepAudioPlayer");
        RunAudioPlayer = GetNode<AudioStreamPlayer>("RunAudioPlayer");
        HitEnemiesAudioPlayer = GetNode<AudioStreamPlayer>("HitEnemiesAudioPlayer");
        HitLevelAudioPlayer = GetNode<AudioStreamPlayer>("HitLevelAudioPlayer");
        HurtAudioPlayer = GetNode<AudioStreamPlayer>("HurtAudioPlayer");
        LaserBeamAudioPlayer = GetNode<AudioStreamPlayer>("LaserBeamAudioPlayer");
        LowEnergyAudioPlayer = GetNode<AudioStreamPlayer>("LowEnergyAudioPlayer");
        SawAudioPlayer = GetNode<AudioStreamPlayer>("SawAudioPlayer");
        ShotAudioPlayer = GetNode<AudioStreamPlayer>("ShotAudioPlayer");
        ChargeLaserAudioPlayer = GetNode<AudioStreamPlayer>("ChargeLaserAudioPlayer"); //Cooldown
        ChargeMineAudioPlayer = GetNode<AudioStreamPlayer>("ChargeMineAudioPlayer");
        StartGameAudioPlayer = GetNode<AudioStreamPlayer>("StartGameAudioPlayer");
        CancelAudioPlayer = GetNode<AudioStreamPlayer>("CancelAudioPlayer");

        GameAudioPlayer = GetNode<AudioStreamPlayer>("GameAudioPlayer");
        BoosRoomAudioPlayer = GetNode<AudioStreamPlayer>("BoosRoomAudioPlayer");
        BossFightAudioPlayer = GetNode<AudioStreamPlayer>("BossFightAudioPlayer");

        enemyExplosions = new AudioStreamPlayer[4];
        enemyExplosions[0] = Explosion0AudioPlayer;
        enemyExplosions[1] = Explosion1AudioPlayer;
        enemyExplosions[2] = Explosion3AudioPlayer;
        enemyExplosions[3] = Explosion4AudioPlayer;
    }

    public void PlayRandomExplosion()
    {
        int index = MoonHunter.Instance.GetRandomInt(3);
        enemyExplosions[index].Play();
    }
}