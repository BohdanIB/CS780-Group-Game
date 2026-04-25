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

	private GameUi _gameUi;
	private GenericGrid<GroundTile> _grid;
	private IsometricTileMap _tileMap;
	private TileMapLayer _currentTileMapLayer; // todo: Potentially expand to other layers depending on hover location?

	private bool _turretPlacerEnabled = false;

	private List<TurretStats> _allTurretStats;

	public void Initialize(GenericGrid<GroundTile> grid, IsometricTileMap tileMap)
	{
		_grid = grid;
		_tileMap = tileMap;

		var layer = tileMap.GetLayers()[0];
		if (layer != null)
			_currentTileMapLayer = layer;
	}

	public void BeginPlacement(int turretIndex)
	{    
		GD.Print($"BeginPlacement called with index {turretIndex}, stats count: {_allTurretStats?.Count}");

		
		if (_allTurretStats == null || _allTurretStats.Count == 0)
			return;

		if (turretIndex < 0 || turretIndex >= _allTurretStats.Count)
			turretIndex = 0;

		_currentTurretIndex = turretIndex;
		_currentTurretTargetMode = TargetingMode.First;
		_turretPlacerEnabled = true;

		_ghostTurret.Initialize(_allTurretStats[_currentTurretIndex]);
		_ghostTurret.UpdateTargetingMode(_currentTurretTargetMode);
		_ghostTurret.Visible = true;
	}

	public override void _Ready()
	{
		_allTurretStats = TurretStats.LoadAllStats();

		if (_allTurretStats != null && _allTurretStats.Count > 0)
		{
			// _ghostTurret.Initialize(TurretStats.ALL_TURRETS[_currentTurretIndex], _currentTurretTargetMode);
		}

		_ghostTurret.DisableTurret();
		_ghostTurret.Visible = false;

		_gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");
		if (_gameUi == null)
			GD.PrintErr("GameUI not found!");

		GD.Print($"Ghost turret node: {_ghostTurret?.Name}, valid: {IsInstanceValid(_ghostTurret)}");
	}

	public override void _Process(double delta)
	{
		if (_currentTileMapLayer == null) return;

		FollowMouse();
		UpdatePlacerState();	
		UpdateGhostTurretState();

		// Toggle existing turret's targeting priority mode
		var tile = GetTile();
		if (Input.IsActionJustPressed("Right Click") && tile != null && tile.Turret != null)
		{
				GD.Print($"PLACING TURRET - stack: {System.Environment.StackTrace}");
				GD.Print($"LEFT CLICK PLACEMENT FIRED - frame: {Engine.GetProcessFrames()}");


			var turret = tile.Turret;
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
			if (_allTurretStats != null && _currentTurretIndex >= _allTurretStats.Count)
			{
				_currentTurretIndex = 0;
			}

			if (_allTurretStats != null && _allTurretStats.Count > 0)
				GD.Print($"Current turret type for placement: {_allTurretStats[_currentTurretIndex].Type}");
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
		if (_turretPlacerEnabled && tile != null && _allTurretStats != null && _allTurretStats.Count > 0)
		{
			// Turret Placement
			var turretStats = _allTurretStats[_currentTurretIndex];

			if (Input.IsActionJustPressed("Left Click"))
			{
				   GD.Print($"LEFT CLICK PLACEMENT FIRED - frame: {Engine.GetProcessFrames()}");
	GD.Print(System.Environment.StackTrace);
				int cost = turretStats.Cost;
				if (_gameUi != null && !_gameUi.TryToSpendCoins(cost))
				{
					_gameUi.ShowWarning("Not enough coins!");
					_turretPlacerEnabled = false;
					return;
				}

				GD.Print($"Placing turret of type {turretStats.Type}");

				var turret = _turretScene.Instantiate<Turret>();
				turret.AddToGroup("placed_turrets");
				tile.Turret = turret;
				// turret.Initialize(turretStats, _currentTurretTargetMode);
				turret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);

			
				var main = GetTree().GetRoot().GetNode("Main");
				main.AddChild(turret);
				
				GD.Print($"Placed turrets in scene: {GetTree().GetNodesInGroup("placed_turrets").Count}");
				turret.HideRadius();

				EmitSignal(SignalName.OnTurretPlaced);
				_turretPlacerEnabled = false;
			}
			else
			{
				// Display "ghost" turret to show where it's going to go and radius
				// Ghost turret hover
				_ghostTurret.Visible = true;
				_ghostTurret.GlobalPosition =
					IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);

				_ghostTurret.UpdateStats(turretStats);
				// GD.Print($"Ghost Turret: {_ghostTurret}");
			}
		}
		else
		{
			_ghostTurret.Visible = false;
		}
	}
}
