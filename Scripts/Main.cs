
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public ulong MAIN_SEED = 12345;

	[Export] private GridRenderer _gridRenderer;
	[Export] private TurretPlacer _turretPlacer;

	[Export] public ConstructionInformation tempConstructionInfo;

	public override void _Ready()
	{
		GD.Seed(MAIN_SEED);

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation);
		PlayArea.instance.grid = grid;
		PlayArea.instance.Render();

		StructurePlacer structurePlacer = GetNode<StructurePlacer>("StructurePlacer");
		structurePlacer.Initialize(grid, tempConstructionInfo, null);

		Enemy.TempEnemyDemo(this, grid, _gridRenderer.TerrainMap, hubLocation);
		Friendly.TempFriendlyDemo(this, grid, _gridRenderer.TerrainMap, hubLocation);
	}
}
