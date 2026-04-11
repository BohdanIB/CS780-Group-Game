using Godot;
using System;


[GlobalClass]
public partial class StructurePlacementRequirements : Resource
{
    /// <summary>
    /// Biomes where the structure may be placed. Leave blank to allow any/all biomes.
    /// </summary>
    [Export] private BiomeType[] _validPlacementBiomes;
    [Export] private BiomeType[] _requiredAdjacentBiomes;

    public bool IsPlacementValid(GenericGrid<GroundTile> placementGrid, Vector2I placementCoordinates)
    {
        GroundTile placementTile = placementGrid.GetGridValueOrDefault(placementCoordinates.X, placementCoordinates.Y);
        if (!IsBiomeValid(placementTile.biome)) return false;

        return true;
    } 


    public bool IsBiomeValid(BiomeType biome)
    {
        if (_validPlacementBiomes == null || _validPlacementBiomes.Length == 0) return true;

        return Array.IndexOf(_validPlacementBiomes, biome) > -1;
    }

    public bool AreAdjacentBiomesValid(BiomeType[] adjacentBiomes)
    {
        foreach (BiomeType biome in _requiredAdjacentBiomes)
        {
            if (Array.IndexOf(adjacentBiomes, biome) <= -1) return false;
        }

        return true;
    }
}
