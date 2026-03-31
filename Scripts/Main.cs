
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public int MAIN_SEED = 12345;

	public override void _Ready()
	{
		Random randomizer = new(); // todo: might just want a global seeded randomizer singleton kicking around somewhere?

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation, MAIN_SEED);

		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");
		gr.RenderGrid(grid);

		//camera stuff
   		var tileSize = gr.terrainMap.TileSet.TileSize;
		var mapWidth = grid.GetWidth() * tileSize.X;
		var mapHeight = grid.GetHeight() * tileSize.Y;

		float zoomX = 1920f / mapWidth;
		float zoomY = 1080f / mapHeight;
		float zoom = Mathf.Min(zoomX, zoomY);
		GD.Print($"zoomX: {zoomX}, zoomY: {zoomY}");
		//float zoom = 3.20f;

		var center = gr.Position + new Vector2(mapWidth / 2f, mapHeight / 2f);

		Camera2D camera = GetNode<Camera2D>("Camera2D");
		camera.Position = center;
		camera.Zoom = new Vector2(zoom, zoom);

		TurretPlacer turretPlacer = GetNode<TurretPlacer>("TurretPlacer");
		turretPlacer.Initialize(grid);

		//Enemy.TempEnemyDemo(this, grid, hubLocation, randomizer);
		EnemySpawner spawner = GetNode<EnemySpawner>("EnemySpawner");
		spawner.Initialize(grid, hubLocation, randomizer);
		Friendly.TempFriendlyDemo(this, grid, hubLocation, randomizer);
	}
}
