using Godot;

public partial class GameUi : CanvasLayer
{
	private Label timerLabel;
	private float elapsedTime = 0f;
	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		GD.Print($"SideMenu found: {sideMenu}");
		
		var turretPlacer = GetTree().GetRoot().GetNode<TurretPlacer>("Main/TurretPlacer");
		GD.Print($"TurretPlacer found: {turretPlacer}");
		
		sideMenu.TurretSelected += (turretType) => {
			GD.Print($"TurretSelected signal received: {turretType}");
			turretPlacer.EnablePlacementMode(turretType);
		};

		// Timer setup
		timerLabel = GetNode<Label>("UI/HBoxContainer/TimerLabel");
		var timer = new Timer();
		timer.WaitTime = 1.0;
		timer.Autostart = true;
		timer.Timeout += OnTimerTick;
		AddChild(timer);
	}

	private void OnTimerTick()
	{
		elapsedTime += 1f;
		int minutes = (int)(elapsedTime / 60);
		int seconds = (int)(elapsedTime % 60);
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}
	
}
