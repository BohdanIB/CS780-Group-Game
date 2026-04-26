using Godot;

public partial class GameSettings : Node
{
    public static GameSettings Instance { get; private set; }

    private const string SAVE_PATH = "user://settings.cfg";
    private ConfigFile _config = new ConfigFile();

    // Enemy Spawner Settings
    public float WaveIntervalSeconds { get; set; } = 30f;
    public int EnemiesPerWave { get; set; } = 5;
    public int FinalWaveNumber { get; set; } = 10;
    public int EnemiesAddedPerWave { get; set; } = 5;

    // Friendly Spawner Settings
    public float FriendlySpawnIntervalSeconds { get; set; } = 10f;
    public int FriendliesPerInterval { get; set; } = 3;

    public override void _Ready()
    {
        Instance = this;
        Load();
    }

    public void Save()
    {
        _config.SetValue("enemy", "wave_interval", WaveIntervalSeconds);
        _config.SetValue("enemy", "enemies_per_wave", EnemiesPerWave);
        _config.SetValue("enemy", "final_wave", FinalWaveNumber);
        _config.SetValue("enemy", "enemies_added", EnemiesAddedPerWave);
        _config.SetValue("friendly", "spawn_interval", FriendlySpawnIntervalSeconds);
        _config.SetValue("friendly", "per_interval", FriendliesPerInterval);
        _config.Save(SAVE_PATH);
    }

    private void Load()
    {
        if (_config.Load(SAVE_PATH) != Error.Ok) return; // No file yet, use defaults

        WaveIntervalSeconds = (float)_config.GetValue("enemy", "wave_interval", WaveIntervalSeconds);
        EnemiesPerWave = (int)_config.GetValue("enemy", "enemies_per_wave", EnemiesPerWave);
        FinalWaveNumber = (int)_config.GetValue("enemy", "final_wave", FinalWaveNumber);
        EnemiesAddedPerWave = (int)_config.GetValue("enemy", "enemies_added", EnemiesAddedPerWave);
        FriendlySpawnIntervalSeconds = (float)_config.GetValue("friendly", "spawn_interval", FriendlySpawnIntervalSeconds);
        FriendliesPerInterval = (int)_config.GetValue("friendly", "per_interval", FriendliesPerInterval);
    }
}