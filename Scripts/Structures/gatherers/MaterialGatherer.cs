using Godot;
using System;

public partial class MaterialGatherer : GenericStructure
{
    public int currentMaterialQuantity;

    public override void Initialize(StructureStats stats)
    {
        _animation.Initialize(stats.Animations, AnimationPackEntry.State.Idle);
    }

}
