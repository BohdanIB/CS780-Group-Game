using Godot;
using System;

public partial class DetectorDetectableParent : Node2D
{
	[Export] public DetectorComponent Detector;
	[Export] public DetectableComponent Detectable;
}
