
using Godot;

/// <summary>
/// Represents a type of tile for a biome. Spawn weight is relative to all other tile types.
/// </br></br>
/// For example: A desert tile has 3 tile types where one has a weight of 8, and the other two types have 
/// a weight of 1. The first tile type has an 80% chance to be picked over the other two types.
/// </summary>
[GlobalClass]
public partial class BiomeTileType : Resource
{
    [Export] public Vector2I GroundTileAtlasCoords = new Vector2I(0,14); // Debug missing tile
    [Export] public uint SpawnWeight = 1; // Larger weights -> greater chance to spawn relative to other tile types
}
