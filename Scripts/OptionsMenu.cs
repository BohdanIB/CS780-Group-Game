using Godot;

public partial class OptionsMenu : Control
{
	private Slider _waveIntervalSlider;
	private Slider _enemiesPerWaveSlider;
	private Slider _finalWaveSlider;
	private Slider _enemiesAddedSlider;
	private Slider _friendlyIntervalSlider;
	private Slider _friendliesPerIntervalSlider;

	private Label _waveIntervalLabel;
	private Label _enemiesPerWaveLabel;
	private Label _finalWaveLabel;
	private Label _enemiesAddedLabel;
	private Label _friendlyIntervalLabel;
	private Label _friendliesPerIntervalLabel;

	public override void _Ready()
	{
		// Enemy settings
		_waveIntervalSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/EnemySection/WaveInterval/HSlider");
		_enemiesPerWaveSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/EnemySection/EnemiesPerWave/HSlider");
		_finalWaveSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/EnemySection/FinalWave/HSlider");
		_enemiesAddedSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/EnemySection/EnemiesAdded/HSlider");

		// Friendly settings
		_friendlyIntervalSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/FriendlySection/FriendlyInterval/HSlider");
		_friendliesPerIntervalSlider = GetNode<Slider>("ScrollContainer/VBoxContainer/FriendlySection/FriendliesPerInterval/HSlider");

		// Labels
		_waveIntervalLabel = GetNode<Label>("ScrollContainer/VBoxContainer/EnemySection/WaveInterval/ValueLabel");
		_enemiesPerWaveLabel = GetNode<Label>("ScrollContainer/VBoxContainer/EnemySection/EnemiesPerWave/ValueLabel");
		_finalWaveLabel = GetNode<Label>("ScrollContainer/VBoxContainer/EnemySection/FinalWave/ValueLabel");
		_enemiesAddedLabel = GetNode<Label>("ScrollContainer/VBoxContainer/EnemySection/EnemiesAdded/ValueLabel");
		_friendlyIntervalLabel = GetNode<Label>("ScrollContainer/VBoxContainer/FriendlySection/FriendlyInterval/ValueLabel");
		_friendliesPerIntervalLabel = GetNode<Label>("ScrollContainer/VBoxContainer/FriendlySection/FriendliesPerInterval/ValueLabel");

		// Load current values
		_waveIntervalSlider.Value = GameSettings.Instance.WaveIntervalSeconds;
		_enemiesPerWaveSlider.Value = GameSettings.Instance.EnemiesPerWave;
		_finalWaveSlider.Value = GameSettings.Instance.FinalWaveNumber;
		_enemiesAddedSlider.Value = GameSettings.Instance.EnemiesAddedPerWave;
		_friendlyIntervalSlider.Value = GameSettings.Instance.FriendlySpawnIntervalSeconds;
		_friendliesPerIntervalSlider.Value = GameSettings.Instance.FriendliesPerInterval;

		UpdateLabels();

		// Connect sliders with auto-save
		_waveIntervalSlider.ValueChanged += (v) => { 
			GameSettings.Instance.WaveIntervalSeconds = (float)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};
		_enemiesPerWaveSlider.ValueChanged += (v) => { 
			GameSettings.Instance.EnemiesPerWave = (int)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};
		_finalWaveSlider.ValueChanged += (v) => { 
			GameSettings.Instance.FinalWaveNumber = (int)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};
		_enemiesAddedSlider.ValueChanged += (v) => { 
			GameSettings.Instance.EnemiesAddedPerWave = (int)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};
		_friendlyIntervalSlider.ValueChanged += (v) => { 
			GameSettings.Instance.FriendlySpawnIntervalSeconds = (float)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};
		_friendliesPerIntervalSlider.ValueChanged += (v) => { 
			GameSettings.Instance.FriendliesPerInterval = (int)v; 
			GameSettings.Instance.Save();
			UpdateLabels(); 
		};

		GetNode<Button>("BackButton").Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/start_menu.tscn");
	}

	private void UpdateLabels()
	{
		_waveIntervalLabel.Text = $"{GameSettings.Instance.WaveIntervalSeconds:F1}s";
		_enemiesPerWaveLabel.Text = $"{GameSettings.Instance.EnemiesPerWave}";
		_finalWaveLabel.Text = $"{GameSettings.Instance.FinalWaveNumber}";
		_enemiesAddedLabel.Text = $"{GameSettings.Instance.EnemiesAddedPerWave}";
		_friendlyIntervalLabel.Text = $"{GameSettings.Instance.FriendlySpawnIntervalSeconds:F1}s";
		_friendliesPerIntervalLabel.Text = $"{GameSettings.Instance.FriendliesPerInterval}";
	}
}