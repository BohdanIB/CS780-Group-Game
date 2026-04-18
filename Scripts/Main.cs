
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public ulong MAIN_SEED = 12345;

	[Export] private GridRenderer _gridRenderer;
	[Export] private TurretPlacer _turretPlacer;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hubLocation = new Vector2I(20, 10);
	

	public override void _Ready()
	{
		GD.Seed(MAIN_SEED);

		_grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), _hubLocation);

		_gridRenderer.RenderGrid(_grid);

		_turretPlacer.Initialize(_grid, _gridRenderer.TerrainMap);

		Enemy.TempEnemyDemo(this, _grid, _gridRenderer.TerrainMap, _hubLocation);
		Friendly.TempFriendlyDemo(this, _grid, _gridRenderer.TerrainMap, _hubLocation);
	}

}
