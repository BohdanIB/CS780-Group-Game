using Godot;
using System;
using System.Collections.Generic;

public partial class TurretPlacer : Node2D
{
	[Signal]
	public delegate void OnTurretPlacedEventHandler();

	[Export] private Turret _ghostTurret;
	[Export] private PackedScene _turretScene;

	private List<TurretStats> _allTurretStats;
	private int _currentTurretIndex = 0;
	private Turret.TargetingMode _currentTurretTargetMode = Turret.TargetingMode.First;

	private GameUi _gameUi;

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
		_allTurretStats = TurretStats.LoadAllStats();

		if (_allTurretStats.Count > 0)
		{
			_ghostTurret.Initialize(_allTurretStats[_currentTurretIndex], _currentTurretTargetMode);
		}

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
				if (!_gameUi.TryToSpendCoins(cost))
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
}
