using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ConstructionInformation : Resource
{
    [Export] public Texture2D DisplayImageAtlas {get; private set;}
    [Export] public Rect2 DisplayImageRect {get; private set;}
    [Export] public StructurePlacementRequirements PlacementRequirements {get; private set;}
	[Export] public MaterialRequirements MaterialRequirements {get; private set;}
    [Export] public PackedScene Structure {get; private set;}
    [Export] public StructureStats StructureStats {get; private set;}
    [Export] public GenericStructure.ConfigurationType ConfigurationType {get; private set;}
}
