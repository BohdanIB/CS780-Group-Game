using Godot;

public partial class CameraController : Camera2D
{
	[Export] public Vector2I HubLocation;

	public override void _Ready()
	{
	
		CallDeferred(nameof(InitializeCamera));
	}

	private void InitializeCamera()
	{
	
		var gridRenderer = GetNode<GridRenderer>("/root/Main/GridRenderer");

		var terrainMap = gridRenderer.TerrainMap;


		var terrainLayers = terrainMap.GetLayers();
		var terrainLayer0 = terrainLayers[0];

	
		Vector2 worldPos = IsometricTileMap.MapCoordToGlobalPosition(terrainLayer0, HubLocation);

		GlobalPosition = worldPos;
	}
}
