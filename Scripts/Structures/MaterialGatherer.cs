using Godot;
using System;

public partial class MaterialGatherer : GenericStructure
{
	[Export] public MaterialType gatheredMaterial;
	public int currentMaterialQuantity;
	[Export] public float gatherTime;

	public override void Initialize(StructureStats stats)
	{
		_animation.Initialize(_animation.Animations, AnimationPackEntry.State.Idle);
	}

}
