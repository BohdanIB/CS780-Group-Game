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
    [Export] private int _minimumPortProximity = -1;

    public bool IsPlacementValid(GenericGrid<GroundTile> placementGrid, Vector2I placementCoordinates)
    {
        GroundTile placementTile = placementGrid.GetGridValueOrDefault(placementCoordinates.X, placementCoordinates.Y);

        if (placementTile.HasRoadConnection() && !_allowWaterPlacement) return false;

        if (!IsBiomeValid(placementTile.biome)) return false;

        
        GroundTile[] adjacentTiles = placementGrid.GetNeighbors(placementCoordinates.X, placementCoordinates.Y, considerDiagonals: false);
        BiomeType[] adjacentBiomes = new BiomeType[4];
        GenericStructure[] adjacentStructures = new GenericStructure[4];

        for (int i = 0; i < 4; i++)
        {
            adjacentBiomes[i] = adjacentTiles[i]?.biome;
            adjacentStructures[i] = adjacentTiles[i]?.Structure;
        }
        if (!AreAdjacentBiomesValid(adjacentBiomes)) return false;
        if (!IsWithinPortRange(adjacentStructures, _minimumPortProximity)) return false;

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

    public bool IsWithinPortRange(GenericStructure[] adjacentStructures, int requiredProximity)
    {
        if (_minimumPortProximity <= 0) return true;
        foreach (GenericStructure structure in adjacentStructures)
        {
            if (structure != null && structure.PortConnectionProximity > requiredProximity) return true;
        }
        return false;
    }
}
