using Godot;
using System;

public partial class Main : Node2D
{
    [Export] public ulong MAIN_SEED = 12345;

	[Export] private StructurePlacer _structurePlacer;
	[Export] public ConstructionInformation tempConstructionInformation;

	private Vector2I _hubLocation;

	public static Inventory PlayerInventory { get; private set; }

	public override void _Ready()
	{
		GD.Randomize();

	

		var coinsMaterial = GD.Load<MaterialType>("res://Resources/Materials/Coins.tres");
		PlayerInventory = new Inventory(); 
		PlayerInventory.AddMaterials(coinsMaterial, 1000);

		GD.Print("PlayerInventory hash: ", PlayerInventory.GetHashCode());

		var camera = GetNode<Camera2D>("/root/Main/Camera2D");
		camera.Enabled = true;
		camera.Zoom = new Vector2(.5f, .5f);

		Vector2I dimensions = new Vector2I(100, 100);

		_hubLocation = new Vector2I(dimensions.X / 2, dimensions.Y / 2);
		GD.Print("Hub location: ", _hubLocation);

		GenericGrid<GroundTile> grid =
			WorldGenerator.GenerateWorldAStar(dimensions, _hubLocation);

		PlayArea.instance.Initialize(grid);
		PlayArea.instance.Render();

		camera.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(
			PlayArea.instance.GridRenderer.TerrainMap.GetLayers()[0],
			_hubLocation
		);

		_structurePlacer.Initialize(PlayArea.instance, PlayerInventory); // ← was: playerInventory

		var enemySpawner = GetNode<EnemySpawner>("EnemySpawner");
		enemySpawner.Initialize(
			grid,
			_hubLocation,
			PlayArea.instance.GridRenderer.TerrainMap.GetLayers()[0]
		);

		var friendlySpawner = GetNode<FriendlySpawner>("FriendlySpawner");
			friendlySpawner.Initialize(
			grid,
			_hubLocation,
			PlayArea.instance.GridRenderer.TerrainMap.GetLayers()[0]
);
	}
}