
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public ulong MAIN_SEED = 12345;

	[Export] private GridRenderer _gridRenderer;
	[Export] private TurretPlacer _turretPlacer;

	public override void _Ready()
	{
		GD.Seed(MAIN_SEED);

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation);

		_gridRenderer.RenderGrid(grid);

		_turretPlacer.Initialize(grid, _gridRenderer.TerrainMap);

		Enemy.TempEnemyDemo(this, grid, _gridRenderer.TerrainMap, hubLocation);
		Friendly.TempFriendlyDemo(this, grid, _gridRenderer.TerrainMap, hubLocation);
	}
}
