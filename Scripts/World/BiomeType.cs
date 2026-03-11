using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class BiomeType : Resource
{
    [Export] public Vector2I groundTileAtlasCoords;
    [Export] public Vector2 humidityGenerationRange = new(0, 1), elevationGenerationRange = new(0, 1), temperatureGenerationRange = new(0, 1);
    [Export] public float generationPriority;
    [Export] public MaterialType[] harvestableMaterials;
}
