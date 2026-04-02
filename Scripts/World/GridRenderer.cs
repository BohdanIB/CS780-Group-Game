using Godot;

public partial class GridRenderer : Node2D
{
	// [Export] public TileMapLayer TopDownTerrainMap, TopDownRoadMap;
	[Export] public IsometricTileMap WaterMap, TerrainMap;

	public void RenderGrid(GenericGrid<GroundTile> grid)
	{
		TerrainMap.Clear();
		WaterMap.Clear();

		// Todo: Just working on 0th layer for now.
		var terrainLayers = TerrainMap.GetLayers();
		var terrainLayer0 = terrainLayers[0];
		var waterLayers = WaterMap.GetLayers();
		var waterLayer0 = waterLayers[0];
		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				GroundTile tile = grid.GetGridValueOrDefault(x, y);

				if (tile == null) continue; // todo: This seems bad, we shouldn't store null tiles in grids and check for null entries.

				// terrainMap.SetCell(new Vector2I(x, y), 0, tile.terrain.groundTileAtlasCoords);

				// int roadIndex = (tile.HasRoadConnection(Vector2I.Up) ? 1 : 0)
				// 			+ (tile.HasRoadConnection(Vector2I.Right) ? 2 : 0)
				// 			+ (tile.HasRoadConnection(Vector2I.Down) ? 4 : 0)
				// 			+ (tile.HasRoadConnection(Vector2I.Left) ? 8 : 0);
				
				// roadMap.SetCell(new Vector2I(x, y), 0, new Vector2I(roadIndex, 0));

				terrainLayer0.SetCell(new Vector2I(x, y), 1, tile.terrain.groundTileAtlasCoords);
				if (tile.HasRoadConnection())
				{
					waterLayer0.SetCell(new Vector2I(x, y), 1, new Vector2I(0,0));
				}

			}
		}

	}

}
