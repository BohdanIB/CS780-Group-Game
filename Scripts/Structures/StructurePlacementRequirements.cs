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
    [Export] private bool _allowWaterPlacement = false;

    public bool IsPlacementValid(GenericGrid<GroundTile> placementGrid, Vector2I placementCoordinates)
    {
        GroundTile placementTile = placementGrid.GetGridValueOrDefault(placementCoordinates.X, placementCoordinates.Y);

        if (placementTile.HasRoadConnection() && !_allowWaterPlacement) return false;

        if (!IsBiomeValid(placementTile.Biome)) return false;

        BiomeType[] adjacentBiomes = new BiomeType[4];
        GroundTile[] adjacentTiles = placementGrid.GetNeighbors(placementCoordinates.X, placementCoordinates.Y, considerDiagonals: false);
        for (int i = 0; i < 4; i++)
        {
            adjacentBiomes[i] = adjacentTiles[i]?.Biome;
        }
        if (!AreAdjacentBiomesValid(adjacentBiomes)) return false;

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
