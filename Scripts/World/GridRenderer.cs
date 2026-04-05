using Godot;

public partial class GridRenderer : Node2D
{
	public TileMapLayer terrainMap, roadMap;

	public override void _Ready()
	{
		terrainMap = GetNode<TileMapLayer>("TerrainMap");
		roadMap = GetNode<TileMapLayer>("RoadMap");
	}

	public void RenderGrid(GenericGrid<GroundTile> grid)
	{
		terrainMap.Clear();
		roadMap.Clear();

		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				GroundTile tile = grid.GetGridValueOrDefault(x, y);

				if (tile == null) continue; // todo: This seems bad, we shouldn't store null tiles in grids and check for null entries.

				terrainMap.SetCell(new Vector2I(x, y), 0, tile._biome.groundTileAtlasCoords);

				int roadIndex = (tile.HasRoadConnection(Vector2I.Up) ? 1 : 0)
							+ (tile.HasRoadConnection(Vector2I.Right) ? 2 : 0)
							+ (tile.HasRoadConnection(Vector2I.Down) ? 4 : 0)
							+ (tile.HasRoadConnection(Vector2I.Left) ? 8 : 0);
				
				roadMap.SetCell(new Vector2I(x, y), 0, new Vector2I(roadIndex, 0));
			}
		}

	}

}
