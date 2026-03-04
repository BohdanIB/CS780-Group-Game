using Godot;
using System;

/// <summary>
/// TODO
/// </summary>
public partial class TurretPlacer : Node2D
{
	private bool _turretPlacerEnabled = false;
	private TurretStats.Category _currentTurretType = TurretStats.Category.Ballista;
	private GenericGrid<GroundTile> _grid;
	private Turret _ghostTurret;
	private Vector2I _currentOriginCoordinates = new();

	[Signal] 
	public delegate void OnTurretPlacedEventHandler();

	public void Initialize(GenericGrid<GroundTile> targetGrid)
	{
		_grid = targetGrid;
	}

	public override void _Ready()
	{
		_ghostTurret = GetNode<Turret>("GhostHoverTurret");
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

		
		if (_turretPlacerEnabled && GetTileIfPlacementValid() is GroundTile tile && tile != null)
		{
			// Turret Placement
			if (Input.IsActionJustPressed("Left Click"))
			{
				GD.Print($"Placing turret of type {_currentTurretType}");
				var turretScene = GD.Load<PackedScene>("res://Scenes/Turret.tscn");
				var turret = turretScene.Instantiate<Turret>();
				tile.Turret = turret;
				turret.Initialize(_currentTurretType);
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
				_ghostTurret.UpdateTurretSprite(baseStats.SpriteFrame);
				_ghostTurret.UpdateTurretRadius(baseStats.AggroRadius);

				// GD.Print($"Ghost Turret: {_ghostTurret}");
				// TODO: Update radius for ghost turret properly
			}
		}
		else
		{
			_ghostTurret.Visible = false;
		}
	}

	private GroundTile GetTileIfPlacementValid()
	{
		// return (_grid.GetGridValueOrDefault(_currentOriginCoordinates.X, _currentOriginCoordinates.Y) is GroundTile tile && 
		// 	tile != null && !tile.HasRoadConnection() && !tile.HasTurret()) ? tile : null;
		if (_grid.GetGridValueOrDefault(_currentOriginCoordinates.X, _currentOriginCoordinates.Y) is GroundTile tile && 
			tile != null && !tile.HasRoadConnection() && !tile.HasTurret())
		{
			// GD.Print($"Turret placement valid for tile: {tile}");
			return tile;
		}
		return null;
		
	}

	private void FollowMouse()
	{
		Vector2 mousePosition = GetViewport().GetMousePosition();
		_currentOriginCoordinates = (Vector2I) (mousePosition / _grid.cellSize).Clamp(Vector2I.Zero, _grid.GetGridDimensions());
		Position = (Vector2) _currentOriginCoordinates * _grid.cellSize;
		// GD.Print($"Current origin position for mouse: {_currentOriginCoordinates}");
	}

}
