using Godot;
using System;

public partial class Main : Node2D
{
    [Export] public ulong MAIN_SEED = 12345;

	[Export] private StructurePlacer _structurePlacer;
	[Export] public ConstructionInformation tempConstructionInformation;

	private Vector2I _hubLocation;

	public override void _Ready()
	{
		GD.Randomize();

	
		var camera = GetNode<Camera2D>("/root/Main/Camera2D");
		camera.Enabled = true;
		//camera.PositionSmoothingEnabled = false;
		camera.Zoom = new Vector2(.5f, .5f);
		//AddChild(camera);

	
		Vector2I dimensions = new Vector2I(100, 100);

	
		_hubLocation = new Vector2I(dimensions.X / 2, dimensions.Y / 2);
		GD.Print("Hub location: ", _hubLocation);

	
		GenericGrid<GroundTile> grid =
			WorldGenerator.GenerateWorldAStar(dimensions, _hubLocation);

		//GD.Print("Grid size: ", grid.GetWidth(), " x ", grid.GetHeight());

		PlayArea.instance.Initialize(grid);
		PlayArea.instance.Render();

	
		camera.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(
			PlayArea.instance.GridRenderer.TerrainMap.GetLayers()[0],
			_hubLocation
		);

	
		_structurePlacer.Initialize(PlayArea.instance, null);

		var enemySpawner = GetNode<EnemySpawner>("EnemySpawner");
		enemySpawner.Initialize(
			grid,
			_hubLocation,
			PlayArea.instance.GridRenderer.TerrainMap.GetLayers()[0]
		);

		Friendly.TempFriendlyDemo(
			this,
			grid,
			PlayArea.instance.GridRenderer.TerrainMap,
			_hubLocation
		);
	}
}
