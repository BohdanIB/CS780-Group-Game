
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public int MAIN_SEED = 12345;

	public override void _Ready()
	{
		Random randomizer = new(MAIN_SEED); // todo: might just want a global seeded randomizer singleton kicking around somewhere?

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation, MAIN_SEED);

		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");
		gr.RenderGrid(grid);

/*added this to make it sync with ui*/
		var tileSize = gr.terrainMap.TileSet.TileSize;
		var center = new Vector2(
		21 * tileSize.X / 2f,
		21 * tileSize.Y / 2f);
		//end of stuff I added
GetNode<Camera2D>("Camera2D").Position = center;

		TurretPlacer turretPlacer = GetNode<TurretPlacer>("TurretPlacer");
		turretPlacer.Initialize(grid);

		Enemy.TempEnemyDemo(this, grid, hubLocation, randomizer);
		Friendly.TempFriendlyDemo(this, grid, hubLocation, randomizer);
	}
}
