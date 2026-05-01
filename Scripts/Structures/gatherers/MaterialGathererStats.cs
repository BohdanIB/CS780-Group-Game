using Godot;
using System;

[GlobalClass]
public partial class MaterialGathererStats : StructureStats
{
    [Export] public MaterialType gatheredMaterial;
    [Export] public float gatherTime;
    [Export] public int gatherQuantity = 1;
}
