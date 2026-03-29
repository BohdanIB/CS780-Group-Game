
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;

/// <summary>
/// TODO
/// </summary>
public partial class TurretPlacer : Node2D
{
	[Signal] 
	public delegate void OnTurretPlacedEventHandler();

	[Export] private Turret _ghostTurret;
	[Export] private PackedScene _turretScene;

	private bool _turretPlacerEnabled = false;
	private TurretStats.Category _currentTurretType = TurretStats.Category.Ballista;
	private TargetingMode _currentTurretTargetMode = TargetingMode.First;
	private GenericGrid<GroundTile> _grid;
	private Vector2I _currentOriginCoordinates = new();

	public void Initialize(GenericGrid<GroundTile> targetGrid)
	{
		_grid = targetGrid;
	}

	public override void _Ready()
	{
		_ghostTurret.Initialize(_currentTurretType);
		// _ghostTurret.UpdateDetectableEntities(Groups.GroupTypes.None);
		// _ghostTurret.UpdateAbleToDetectEntities(Groups.GroupTypes.None);
		_ghostTurret.DisableTurret();
		_ghostTurret.Visible = false;
	}

	public override void _Process(double delta)
	{
		FollowMouse();
		UpdateState();
		UpdateGhostTurretState();

		// Toggle turret targeting priority mode
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
		Vector2 mousePosition = GetViewport().GetMousePosition();
		_currentOriginCoordinates = (Vector2I) (mousePosition / _grid.cellSize).Clamp(Vector2I.Zero, _grid.GetGridDimensions());
		Position = (Vector2) _currentOriginCoordinates * _grid.cellSize;
		// GD.Print($"Current origin position for mouse: {_currentOriginCoordinates}");
	}

	private void UpdateState()
	{
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
			// Turret Placement
			if (Input.IsActionJustPressed("Left Click"))
			{
				GD.Print($"Placing turret of type {_currentTurretType}");
				var turret = _turretScene.Instantiate<Turret>();
				tile.Turret = turret;
				turret.Initialize(_currentTurretType, _currentTurretTargetMode);
				turret.GlobalPosition = _grid.GetCentralGridCellPositionPixels(tile.position);
				GetTree().GetRoot().AddChild(turret);
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
	}

}
