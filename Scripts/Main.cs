using Godot;
using System;

public partial class Main : Node2D
{
	[Export] public ulong MAIN_SEED = 12345;

	[Export] private GridRenderer _gridRenderer;
	[Export] private TurretPlacer _turretPlacer;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hubLocation = new Vector2I(20, 10);
	

	public override void _Ready()
	{
		Random randomizer = new Random();
		MAIN_SEED = (ulong)randomizer.NextInt64();
		GD.Seed(MAIN_SEED);

		var hubLocation = new Vector2I(50, 50);

		GenericGrid<GroundTile> grid =
			WorldGenerator.GenerateWorldAStar(
				new Vector2I(100, 100),
				hubLocation
			);
		_grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), _hubLocation);

		_gridRenderer.RenderGrid(grid);

		var tileSize = new Vector2I(1, 1);
		var mapWidth = grid.GetWidth() * tileSize.X;
		var mapHeight = grid.GetHeight() * tileSize.Y;

		float zoomX = 1920f / mapWidth;
		float zoomY = 1080f / mapHeight;
		float zoom = Mathf.Min(zoomX, zoomY);

		GD.Print($"zoomX: {zoomX}, zoomY: {zoomY}");

		var center = _gridRenderer.Position +
					 new Vector2(mapWidth / 2f, mapHeight / 2f);

		Camera2D camera = GetNode<Camera2D>("Camera2D");

		_turretPlacer.Initialize(_grid, _gridRenderer.TerrainMap);

		EnemySpawner spawner = GetNode<EnemySpawner>("EnemySpawner");
		spawner.Initialize(grid, hubLocation, randomizer, _gridRenderer.TerrainMap.GetLayers()[0]);

		Friendly.TempFriendlyDemo(this, grid, _gridRenderer.TerrainMap, hubLocation);
	}

}
