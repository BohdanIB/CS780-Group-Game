using Godot;
using System;

/// <summary>
/// TODO
/// </summary>
public partial class TurretPlacer : Node2D
{
	private bool _turretPlacerEnabled = false;
	private TurretStats.Category _currentTurretType = TurretStats.Category.Ballista;
	private Turret.TargetingMode _currentTurretTargetMode = Turret.TargetingMode.First;
	private GenericGrid<GroundTile> _grid;
	private Vector2I _currentOriginCoordinates = new();

	// Scene Children
	[Export] private Turret _ghostTurret;

	// Preloaded Scenes
	[Export] private PackedScene _turretScene;

	[Signal] 
	public delegate void OnTurretPlacedEventHandler();
	private GameUi _gameUi;

	public void Initialize(GenericGrid<GroundTile> targetGrid)
	{
		_grid = targetGrid;
	}

	public override void _Ready()
	{
		SetProcessInput(true);
		SetProcessUnhandledInput(true);
		_ghostTurret = GetNode<Turret>("GhostHoverTurret");
		_ghostTurret.Initialize(_currentTurretType);
		_ghostTurret.Visible = false;
		_gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");
	}

	public override void _Process(double delta)
	{
		FollowMouse();

		if (Input.IsActionJustPressed("ToggleTurretPlacementMode"))
		{
			_turretPlacerEnabled = !_turretPlacerEnabled;
			GD.Print($"TurretPlacer {(_turretPlacerEnabled ? "enabled" : "disabled")}");
		}

		if (Input.IsActionJustPressed("SwitchTurretType"))
		{
			_currentTurretType++;
			if (!Enum.IsDefined(typeof(TurretStats.Category), _currentTurretType))
			{
				_currentTurretType = 0;
			}
			GD.Print($"Current turret type for placement: {_currentTurretType}");
		}
		if (Input.IsActionJustPressed("SwitchTurretTargetingMode"))
		{
			_currentTurretTargetMode++;
			if (!Enum.IsDefined(typeof(Turret.TargetingMode), _currentTurretTargetMode))
			{
				_currentTurretTargetMode = 0;
			}
			GD.Print($"Current turret target mode for placement and changing: {_currentTurretTargetMode}");
		}

		GroundTile tile;

		tile = GetTileIfStructurePlacementValid();
		if (_turretPlacerEnabled && tile != null)
		{
			// Turret Placement
			if (Input.IsActionJustPressed("Left Click"))
			{

				int cost = TurretStats.GetBaseTurretStats(_currentTurretType).Cost;
				 if (!_gameUi.TryToSpendCoins(cost))
   				 {
					//GD.Print("Not enough coins!");
					_gameUi.ShowWarning("Not enough coins!");
					_turretPlacerEnabled = false;
					return;
				}

				GD.Print($"Placing turret of type {_currentTurretType}");
				var turret = _turretScene.Instantiate<Turret>();
				turret.AddToGroup("placed_turrets");
				tile.Turret = turret;
				turret.Initialize(_currentTurretType, _currentTurretTargetMode);
				turret.GlobalPosition = _grid.GetCentralGridCellPositionPixels(tile.position);
				GetTree().GetRoot().AddChild(turret);
				EmitSignal(SignalName.OnTurretPlaced);
				_turretPlacerEnabled = false;
			}
			// Display "ghost" turret to show where it's going to go and radius
			// Ghost turret hover
			else
			{
				_ghostTurret.Visible = true;
				_ghostTurret.GlobalPosition = _grid.GetCentralGridCellPositionPixels(tile.position);
				TurretStats baseStats = TurretStats.GetBaseTurretStats(_currentTurretType);
				_ghostTurret.UpdateStats(baseStats);
				// GD.Print($"Ghost Turret: {_ghostTurret}");
			}
		}
		else
		{
			_ghostTurret.Visible = false;
		}

		// Toggle turret targeting priority mode
		tile = GetTile();
		if (Input.IsActionJustPressed("Right Click") && tile != null && tile.Turret != null)
		{
			var turret = tile.Turret;
			GD.Print($"Updating turret {turret.Name} targeting mode to {_currentTurretTargetMode}");
			turret.UpdateTargetingMode(_currentTurretTargetMode);
		}
	}

	private GroundTile GetTileIfStructurePlacementValid()
	{
		if (_grid.GetGridValueOrDefault(_currentOriginCoordinates.X, _currentOriginCoordinates.Y) is GroundTile tile && 
			tile != null && !tile.HasRoadConnection() && !tile.HasStructure())
		{
			// GD.Print($"Structure placement valid for tile: {tile}");
			return tile;
		}
		return null;
	}

	/// <summary>
	/// Can return null
	/// </summary>
	/// <returns></returns>
	private GroundTile GetTile()
	{
		return _grid.GetGridValueOrDefault(_currentOriginCoordinates.X, _currentOriginCoordinates.Y);
	}

	private void FollowMouse()
	{
		// Correct world-space mouse position
		Vector2 worldMouse = GetViewport().GetCamera2D().GetGlobalMousePosition();

		// Convert world → grid
   		 _currentOriginCoordinates = (Vector2I)(worldMouse / _grid.cellSize)
		.Clamp(Vector2I.Zero, _grid.GetGridDimensions());

		// Move the ghost turret node to the snapped world position
		Position = (Vector2)_currentOriginCoordinates * _grid.cellSize;
		//GD.Print($"Mouse world: {worldMouse}, grid: {_currentOriginCoordinates}");

	}

	public void EnablePlacementMode(TurretStats.Category turretType)
	{
		_currentTurretType = turretType;
		_turretPlacerEnabled = true;
		GD.Print($"TurretPlacer enabled for {turretType}");
	}

}
