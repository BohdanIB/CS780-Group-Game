using Godot;

public partial class GameUi : CanvasLayer
{
	private Label timerLabel;
	private float elapsedTime = 0f;
	private Label killCountLabel;
	private int enemyKillCount = 0;
	//private int coins = 1000;
	private Label coinCountLabel;
	private Label _warningLabel;	
	private Label _BaseHPLabel;

	public const int StartingBaseHP = 100;
	private int currentBaseHP = StartingBaseHP;

	private bool game_over = false;

	private MaterialType _coinsMaterial;

	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		_warningLabel = GetNode<Label>("UI/WarningLabel");

		_coinsMaterial = GD.Load<MaterialType>("res://Resources/Materials/Coins.tres");
		
		var structurePlacer = GetNode<StructurePlacer>("../StructurePlacer");
		coinCountLabel = GetNode<Label>("UI/Panel/HBoxContainer/CoinCount/CoinCountLabel");

		_BaseHPLabel = GetNode<Label>("UI/Panel/HBoxContainer/BaseHealth/BaseHPLabel");
		_BaseHPLabel.Text = $"{StartingBaseHP}";
		
		killCountLabel = GetNode<Label>("UI/Panel/HBoxContainer/KillCount/KillCountLabel");
		coinCountLabel.Text = "1000";

		
		
		sideMenu.TurretSelected += (turretType) =>
		{
			structurePlacer.SetStructure(
				structurePlacer.temporaryConstructionInfo[(int)turretType]
			);
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
		timerLabel = GetNode<Label>("UI/Panel/HBoxContainer/TimerLabel");
		var timer = new Timer();
		timer.WaitTime = 1.0;
		timer.Autostart = true;
		timer.Timeout += OnTimerTick;
		AddChild(timer);
	}

		public void IncrementKillCount()
	{
		enemyKillCount++;
		AddCoins(25);
		killCountLabel.Text = $"Kills: {enemyKillCount}";
	}


	public void ShowVictory()
	{
		game_over = true;

		var screen = GetNode<GameOverScreen>("GameOverScreen");
		screen.onVictory();   
	}

	public void UpdateCoinDisplay()
{
	coinCountLabel.Text = $"{Main.PlayerInventory.GetMaterialCount(_coinsMaterial)}";
}

	

	public void ShowWarning(string message , bool GameOver = false)
	{
		_warningLabel.Text = message;
		_warningLabel.Visible = true;
		
		if(!GameOver){
			
			var timer = GetTree().CreateTimer(3.0f); // show for 3 seconds
			timer.Timeout += () => _warningLabel.Visible = false;
		}
		else
		{
			game_over = true; 
			GetNode<GameOverScreen>("GameOverScreen").onGameOver();

		}
	}

	public bool TryToSpendCoins(int amount)
	{
		if (!Main.PlayerInventory.HasMaterial(_coinsMaterial, amount)) return false;
		Main.PlayerInventory.RemoveMaterials(_coinsMaterial, amount);
		coinCountLabel.Text = $"{Main.PlayerInventory.GetMaterialCount(_coinsMaterial)}";
		return true;
	}

		public void AddCoins(int amount)
	{
		Main.PlayerInventory.AddMaterials(_coinsMaterial, amount);
		coinCountLabel.Text = $"{Main.PlayerInventory.GetMaterialCount(_coinsMaterial)}";
	}

	public void TakeDamage(int amount)
	{
		if (game_over){return;}
		
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
