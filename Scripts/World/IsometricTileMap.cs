
using Godot;

public partial class IsometricTileMap : Node2D
{
	[Export] private TileMapLayer[] _layers;

	public TileMapLayer[] GetLayers()
	{
		return _layers;
	}

	/// <summary>
	/// Clear all layers in terrain map
	/// </summary>
	public void Clear()
	{
		foreach (var layer in _layers)
		{
			layer.Clear();
		}
	}

	/// <summary>
	/// Converts a global position into a map coordinate for the given tile map layer.
	/// </summary>
	/// <param name="tileMap"></param>
	/// <param name="pos">global position</param>
	/// <returns></returns>
	public static Vector2I GlobalPositionToMapCoord(TileMapLayer tileMap, Vector2 pos)
	{
		return tileMap.LocalToMap(tileMap.ToLocal(pos));
	}
	/// <summary>
	/// Converts a given map coordinate into an associated global position. The global position 
	/// is the center point of the tile on the given tile map layer.
	/// </summary>
	/// <param name="tileMap"></param>
	/// <param name="coord"></param>
	/// <returns></returns>
	public static Vector2 MapCoordToGlobalPosition(TileMapLayer tileMap, Vector2I coord)
	{
		return tileMap.ToGlobal(tileMap.MapToLocal(coord));
	}

	/// <summary>
	/// Takes a global point and finds the nearest centerpoint of a tile on the tilemap.
	/// </summary>
	/// <param name="tileMap"></param>
	/// <param name="pos">global position</param>
	/// <returns></returns>
	public static Vector2 CenterTilePosition(TileMapLayer tileMap, Vector2 pos)
	{
		return MapCoordToGlobalPosition(tileMap, GlobalPositionToMapCoord(tileMap, pos));
	}

}
