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
	private IsometricTileMap _tileMap;
	private TileMapLayer _currentTileMapLayer; // todo: Potentially expand to other layers depending on hover location?
	// private Vector2I _currentOriginCoordinates = new();

	// Scene Children
	[Export] private Turret _ghostTurret;

	// Preloaded Scenes
	[Export] private PackedScene _turretScene;

	[Signal] 
	public delegate void OnTurretPlacedEventHandler();

	public void Initialize(GenericGrid<GroundTile> grid, IsometricTileMap tileMap)
	{
		_grid = grid;
		_tileMap = tileMap;
		var layer = tileMap.GetLayers()[0];
		if (layer != null)
		{
			_currentTileMapLayer = layer;
		}
	}

	public override void _Ready()
	{
		_ghostTurret.Initialize(_currentTurretType);
		_ghostTurret.Visible = false;
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
				GD.Print($"Placing turret of type {_currentTurretType}");
				var turret = _turretScene.Instantiate<Turret>();
				tile.Turret = turret;
				turret.Initialize(_currentTurretType, _currentTurretTargetMode);
				turret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);
				GetTree().GetRoot().AddChild(turret);
			}
			// Display "ghost" turret to show where it's going to go and radius
			// Ghost turret hover
			else
			{
				_ghostTurret.Visible = true;
				_ghostTurret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);
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
		var coord = IsometricTileMap.GlobalPositionToMapCoord(_currentTileMapLayer, GlobalPosition);
		if (_grid.GetGridValueOrDefault(coord.X, coord.Y) is GroundTile tile && 
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
		var coord = IsometricTileMap.GlobalPositionToMapCoord(_currentTileMapLayer, GlobalPosition);
		return _grid.GetGridValueOrDefault(coord.X, coord.Y);
	}

	private void FollowMouse()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		// _currentOriginCoordinates = (Vector2I) (mousePosition / _grid.cellSize).Clamp(Vector2I.Zero, _grid.GetGridDimensions());
		GlobalPosition = IsometricTileMap.CenterTilePosition(_currentTileMapLayer, mousePosition);
		// GD.Print($"Current origin position for mouse: {_currentOriginCoordinates}");
		// GD.Print($"Coordinate: {IsometricTileMap.GlobalPositionToMapCoord(_currentTileMapLayer, mousePosition)} - Global Position: {IsometricTileMap.CenterTilePosition(_currentTileMapLayer, mousePosition)}");
	}

}
