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
	private Label _BaseHPLabel;

	public const int StartingBaseHP = 100;
	private int currentBaseHP = StartingBaseHP;

	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		_warningLabel = GetNode<Label>("UI/WarningLabel");
		
		var turretPlacer = GetTree().GetRoot().GetNode<TurretPlacer>("Main/TurretPlacer");
		coinCountLabel = GetNode<Label>("UI/HBoxContainer/CoinCount/CoinCountLabel");

		_BaseHPLabel = GetNode<Label>("UI/HBoxContainer/BaseHealth/BaseHPLabel");
		_BaseHPLabel.Text = $"{StartingBaseHP}";
		
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


		_warningLabel.MouseFilter = Control.MouseFilterEnum.Ignore;
		var styleBox = new StyleBoxFlat();
		styleBox.BgColor = new Color(0, 0, 0, 0.7f);
		styleBox.ContentMarginLeft = 10;
		styleBox.ContentMarginRight = 10;
		styleBox.ContentMarginTop = 5;
		styleBox.ContentMarginBottom = 5;
		_warningLabel.Visible = false;
		_warningLabel.AddThemeStyleboxOverride("normal", styleBox);

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
		coins += 25;
		coinCountLabel.Text = $"{coins}";
		killCountLabel.Text = $"Kills: {enemyKillCount}";

	}

	public void ShowWarning(string message , bool GameOver = false)
	{
		_warningLabel.Text = message;
		_warningLabel.Visible = true;
		if(!GameOver){
			var timer = GetTree().CreateTimer(3.0f); // show for 3 seconds
			timer.Timeout += () => _warningLabel.Visible = false;
		}
	}

	public bool TryToSpendCoins(int amount)
	{
		if (coins < amount) return false;
		
		coins -= amount;
		coinCountLabel.Text = $"{coins}";
		return true;
	}

	public void TakeDamage(int amount)
	{
		currentBaseHP -= amount;
		_BaseHPLabel.Text = $"{currentBaseHP}";
	if (currentBaseHP <= 0)
	{
		ShowWarning("Base destroyed! Game Over!", GameOver: true);
	}
}

	private void OnTimerTick()
	{
		elapsedTime += 1f;
		int minutes = (int)(elapsedTime / 60);
		int seconds = (int)(elapsedTime % 60);
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}
	
}
