
using Godot;

[GlobalClass]
public partial class BiomeType : Resource
{
	[Export] private BiomeTileType[] TileTypes = [];
	[Export] public Vector2 humidityGenerationRange = new(0, 1), elevationGenerationRange = new(0, 1), temperatureGenerationRange = new(0, 1);
	[Export] public float generationPriority;
	[Export] public MaterialType[] harvestableMaterials;

	/// <summary>
	/// Gets a tile given weighted spawn values. Weights are all relative to each other.
	/// </summary>
	/// <returns></returns>
	public BiomeTileType GetTile()
	{
		if (TileTypes.Length <= 0) { return DebugTile(); }
		var pickedTile = TileTypes[0];

		uint totalWeight = 0;
		foreach (var tile in TileTypes)
		{
			totalWeight += tile.SpawnWeight;
		}
		var selectedTileWeightValue = GD.Randi() % totalWeight;

		uint upperWeightRangeExclusive = 0;
		foreach (var tile in TileTypes)
		{
			upperWeightRangeExclusive += tile.SpawnWeight;
			if (selectedTileWeightValue < upperWeightRangeExclusive)
			{
				pickedTile = tile;
				break;
			}
		}
		return pickedTile;
	}

	/// <summary>
	/// It's assumed that the 0th TileType is the "default" clear tile for a biome.
	/// </summary>
	public BiomeTileType GetDefaultTile()
	{
		if (TileTypes.Length <= 0) { return DebugTile(); }

		return TileTypes[0];
	}

	private BiomeTileType DebugTile()
	{
		return new BiomeTileType();
	}
}
