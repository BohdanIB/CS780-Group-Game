using Godot;

public partial class GameUi : CanvasLayer
{
	private const int COIN_MATERIAL_INDEX = 0;
	[Export] public Label[] materialDisplays;
	[Export] public MaterialType[] materialsToDisplay;

	private Label timerLabel;
	private float elapsedTime = 0f;
	private Label killCountLabel;
	private int enemyKillCount = 0;

	private Label _warningLabel;	
	private Label _BaseHPLabel;

	public const int StartingBaseHP = 1000;
	private int currentBaseHP = StartingBaseHP;

	private bool game_over = false;

	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		_warningLabel = GetNode<Label>("UI/WarningLabel");

		
		var structurePlacer = GetNode<StructurePlacer>("../StructurePlacer");


		_BaseHPLabel = GetNode<Label>("UI/Panel/HBoxContainer/BaseHealth/BaseHPLabel");
		_BaseHPLabel.Text = $"{StartingBaseHP}";
		
		killCountLabel = GetNode<Label>("UI/Panel/HBoxContainer/KillCount/KillCountLabel");

		
		
		sideMenu.StructureSelected += structurePlacer.SetStructure;


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

	public void UpdateMaterialDisplays()
	{
		for (int i = 0; i < materialsToDisplay.Length; i++)
		{
			materialDisplays[i].Text = Main.PlayerInventory.GetMaterialCount(materialsToDisplay[i]) + "";
		}
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

		public void AddCoins(int amount)
	{
		Main.PlayerInventory.AddMaterials(materialsToDisplay[COIN_MATERIAL_INDEX], amount);
		UpdateMaterialDisplays();
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
