using Godot;
using System;

[GlobalClass]
public partial class TerrainType : Resource
{
    [Export] public Vector2I groundTileAtlasCoords;
}
