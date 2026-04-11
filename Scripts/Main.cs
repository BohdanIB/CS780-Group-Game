
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public ulong MAIN_SEED = 12345;


	[Export] private StructurePlacer _structurePlacer;

	[Export] public ConstructionInformation tempConstructionInformation;

	public override void _Ready()
	{
		GD.Seed(MAIN_SEED);

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation);
		PlayArea.instance.Initialize(grid);
		PlayArea.instance.Render();

		_structurePlacer.Initialize(PlayArea.instance, null);
		_structurePlacer.SetStructure(tempConstructionInformation);

		Enemy.TempEnemyDemo(this, grid, PlayArea.instance.GridRenderer.TerrainMap, hubLocation);
		Friendly.TempFriendlyDemo(this, grid, PlayArea.instance.GridRenderer.TerrainMap, hubLocation);
	}
}
