using Godot;

public partial class GameUi : CanvasLayer
{
	private Label timerLabel;
	private float elapsedTime = 0f;
	private Label killCountLabel;
	private int enemyKillCount = 0;
	private int coins = 1000;
	private Label coinCountLabel;
	private Label _warningLabel;	

	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		_warningLabel = GetNode<Label>("UI/WarningLabel");
		
		var turretPlacer = GetTree().GetRoot().GetNode<TurretPlacer>("Main/TurretPlacer");
		coinCountLabel = GetNode<Label>("UI/HBoxContainer/CoinCount/CoinCountLabel");
		
		killCountLabel = GetNode<Label>("UI/HBoxContainer/KillCount/KillCountLabel");
		coinCountLabel.Text = $"{coins}";
		
		
		sideMenu.TurretSelected += (turretType) => {
			//GD.Print($"TurretSelected signal received: {turretType}");
			turretPlacer.EnablePlacementMode(turretType);
		};
		turretPlacer.OnTurretPlaced += () =>{
			coins -= 100; 
			coinCountLabel.Text = $"{coins}";
		};

		// Timer setup
		timerLabel = GetNode<Label>("UI/HBoxContainer/TimerLabel");
		var timer = new Timer();
		timer.WaitTime = 1.0;
		timer.Autostart = true;
		timer.Timeout += OnTimerTick;
		AddChild(timer);
	}

	public void IncrementKillCount()
	{
		enemyKillCount++;
		killCountLabel.Text = $"Kills: {enemyKillCount}";

	}

	public void ShowWarning(string message)
	{
		_warningLabel.Text = message;
		_warningLabel.Visible = true;
		 var timer = GetTree().CreateTimer(2.0f); // show for 2 seconds
		timer.Timeout += () => _warningLabel.Visible = false;
	}

	public bool TryToSpendCoins(int amount)
	{
		if (coins < amount) return false;
		
		coins -= amount;
		coinCountLabel.Text = $"{coins}";
		return true;
	}

	private void OnTimerTick()
	{
		elapsedTime += 1f;
		int minutes = (int)(elapsedTime / 60);
		int seconds = (int)(elapsedTime % 60);
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}
	
}
