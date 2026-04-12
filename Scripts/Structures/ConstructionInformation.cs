using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ConstructionInformation : Resource
{
    [Export] public Texture2D DisplayImage {get; private set;}
    [Export] public StructurePlacementRequirements PlacementRequirements {get; private set;}
	[Export] public MaterialRequirements MaterialRequirements {get; private set;}
    [Export] public PackedScene Structure {get; private set;}
    [Export] public GenericStructure.ConfigurationType ConfigurationType {get; private set;}
}
