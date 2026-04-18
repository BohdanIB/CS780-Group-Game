
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;

public partial class TurretPlacer : Node2D
{
	[Signal]
	public delegate void OnTurretPlacedEventHandler();

	[Export] private Turret _ghostTurret;
	[Export] private PackedScene _turretScene;

	private int _currentTurretIndex = 0;
	private Turret.TargetingMode _currentTurretTargetMode = Turret.TargetingMode.First;

	private GameUi _gameUi;
	private TargetingMode _currentTurretTargetMode = TargetingMode.First;

	private GenericGrid<GroundTile> _grid;
	private IsometricTileMap _tileMap;
	private TileMapLayer _currentTileMapLayer;

	private bool _turretPlacerEnabled = false;

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
		if (_allTurretStats == null || _allTurretStats.Count == 0)
			return;

		if (turretIndex < 0 || turretIndex >= _allTurretStats.Count)
			turretIndex = 0;

		_currentTurretIndex = turretIndex;
		_currentTurretTargetMode = Turret.TargetingMode.First;
		_turretPlacerEnabled = true;

		_ghostTurret.Initialize(_allTurretStats[_currentTurretIndex], _currentTurretTargetMode);
		_ghostTurret.Visible = true;
	}

	public override void _Ready()
	{
		if (TurretStats.ALL_TURRETS.Count > 0)
		_allTurretStats = TurretStats.LoadAllStats();

		if (_allTurretStats.Count > 0)
		{
			_ghostTurret.Initialize(TurretStats.ALL_TURRETS[_currentTurretIndex], _currentTurretTargetMode);
		}
		
		_ghostTurret.DisableTurret();

		_ghostTurret.Visible = false;

		_gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");
		if (_gameUi == null) GD.PrintErr("GameUI not found!");
	}

	public override void _Process(double delta)
	{
		if (_currentTileMapLayer == null) return;

		FollowMouse();
		UpdatePlacerState();
		UpdateGhostTurretState();

		// Toggle existing turret's targeting priority mode
		var tile = GetTile();

		if (Input.IsActionJustPressed("ToggleTurretPlacementMode"))
		{
			_turretPlacerEnabled = !_turretPlacerEnabled;
			GD.Print($"TurretPlacer {(_turretPlacerEnabled ? "enabled" : "disabled")}");
		}

		if (Input.IsActionJustPressed("SwitchTurretType"))
		{
			_currentTurretIndex++;
			if (_currentTurretIndex >= _allTurretStats.Count)
				_currentTurretIndex = 0;

			GD.Print($"Current turret type for placement: {_allTurretStats[_currentTurretIndex].Type}");
		}

		if (Input.IsActionJustPressed("SwitchTurretTargetingMode"))
		{
			_currentTurretTargetMode++;
			if (!Enum.IsDefined(typeof(Turret.TargetingMode), _currentTurretTargetMode))
				_currentTurretTargetMode = 0;

			GD.Print($"Current turret target mode: {_currentTurretTargetMode}");
		}

		GroundTile tile = GetTileIfStructurePlacementValid();

		if (_turretPlacerEnabled && tile != null)
		{
			var turretStats = _allTurretStats[_currentTurretIndex];

			if (Input.IsActionJustPressed("Left Click"))
			{
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

				turret.Initialize(turretStats, _currentTurretTargetMode);
				turret.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);

				GetTree().GetRoot().AddChild(turret);
				turret.HideRadius();

				EmitSignal(SignalName.OnTurretPlaced);
				_turretPlacerEnabled = false;
			}
			else
			{
				_ghostTurret.Visible = true;
				_ghostTurret.GlobalPosition =
					IsometricTileMap.MapCoordToGlobalPosition(_currentTileMapLayer, tile.position);

				_ghostTurret.UpdateStats(turretStats);
			}
		}
		else
		{
			_ghostTurret.Visible = false;
		}

		tile = GetTile();
		if (Input.IsActionJustPressed("Right Click") && tile != null && tile.Turret != null)
		{
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
			return tile;
		}

		return null;
	}

	private GroundTile GetTile()
	{
		var coord = IsometricTileMap.GlobalPositionToMapCoord(_currentTileMapLayer, GlobalPosition);
		return _grid.GetGridValueOrDefault(coord.X, coord.Y);
	}

	private void FollowMouse()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		GlobalPosition = IsometricTileMap.CenterTilePosition(_currentTileMapLayer, mousePosition);
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
				turret.Initialize(turretStats, _currentTurretTargetMode);
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
