using Godot;
using System;

public partial class TargetingTestScene : Node2D
{
    [Export] public TargetingComponent TargetingComponent;
    [Export] public DetectorComponent DetectorComponent;
    // Target
    [Export] public DetectableComponent DetectableComponent;
}
