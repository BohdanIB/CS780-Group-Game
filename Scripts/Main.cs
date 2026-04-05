
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public int MAIN_SEED = 12345;

	[Export] public ConstructionInformation tempConstructionInfo;

	public override void _Ready()
	{
		Random randomizer = new(MAIN_SEED); // todo: might just want a global seeded randomizer singleton kicking around somewhere?

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation, MAIN_SEED);
		PlayArea.instance.grid = grid;
		PlayArea.instance.Render();

		StructurePlacer structurePlacer = GetNode<StructurePlacer>("StructurePlacer");
		structurePlacer.Initialize(grid, tempConstructionInfo, null);

		Enemy.TempEnemyDemo(this, grid, hubLocation, randomizer);
		Friendly.TempFriendlyDemo(this, grid, hubLocation, randomizer);
	}
}
