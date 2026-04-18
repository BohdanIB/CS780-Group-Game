
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// TODO
/// </summary>
public partial class TurretPlacer : Node2D
{
	[Signal] 
	public delegate void OnTurretPlacedEventHandler();

	// Scene Children
	[Export] private Turret _ghostTurret;

	// Preloaded Scenes
	[Export] private PackedScene _turretScene;

	private int _currentTurretIndex = 0;
	private TargetingMode _currentTurretTargetMode = TargetingMode.First;

	private GenericGrid<GroundTile> _grid;
	private IsometricTileMap _tileMap;
	private TileMapLayer _currentTileMapLayer; // todo: Potentially expand to other layers depending on hover location?

	private bool _turretPlacerEnabled = false;

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
		if (TurretStats.ALL_TURRETS.Count > 0)
		{
			// _ghostTurret.Initialize(TurretStats.ALL_TURRETS[_currentTurretIndex], _currentTurretTargetMode);
		}
		
		_ghostTurret.DisableTurret();
		_ghostTurret.Visible = false;
	}

	public override void _Process(double delta)
	{
		FollowMouse();
		UpdatePlacerState();
		UpdateGhostTurretState();

		// Toggle existing turret's targeting priority mode
		var tile = GetTile();
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

	private void UpdatePlacerState()
	{
		if (Input.IsActionJustPressed("ToggleTurretPlacementMode"))
		{
			_turretPlacerEnabled = !_turretPlacerEnabled;
			GD.Print($"TurretPlacer {(_turretPlacerEnabled ? "enabled" : "disabled")}");
		}

		if (Input.IsActionJustPressed("SwitchTurretType"))
		{
			_currentTurretIndex++;
			if (_currentTurretIndex >= TurretStats.ALL_TURRETS.Count)
			{
				_currentTurretIndex = 0;
			}
			GD.Print($"Current turret type for placement: {TurretStats.ALL_TURRETS[_currentTurretIndex].Type}");
		}
		if (Input.IsActionJustPressed("SwitchTurretTargetingMode"))
		{
			_currentTurretTargetMode++;
			if (!Enum.IsDefined(typeof(TargetingMode), _currentTurretTargetMode))
			{
				_currentTurretTargetMode = 0;
			}
			GD.Print($"Current turret target mode for placement and changing: {_currentTurretTargetMode}");
		}

	}

	private void UpdateGhostTurretState()
	{
		GroundTile tile = GetTileIfStructurePlacementValid();
		if (_turretPlacerEnabled && tile != null)
		{
			var turretStats = TurretStats.ALL_TURRETS[_currentTurretIndex];
			// Turret Placement
			if (Input.IsActionJustPressed("Left Click"))
			{
				GD.Print($"Placing turret of type {turretStats.Type}");
				var turret = _turretScene.Instantiate<Turret>();
				tile.Turret = turret;
				// turret.Initialize(turretStats, _currentTurretTargetMode);
				turret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);
				GetTree().GetRoot().AddChild(turret);
			}
			// Display "ghost" turret to show where it's going to go and radius
			// Ghost turret hover
			else
			{
				_ghostTurret.Visible = true;
				_ghostTurret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);
				_ghostTurret.UpdateStats(turretStats); // todo
				// GD.Print($"Ghost Turret: {_ghostTurret}");
			}
		}
		else
		{
			_ghostTurret.Visible = false;
		}
	}

}
