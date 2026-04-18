using Godot;
using System;

public partial class DetectorDetectableTestScene : Node2D
{
	/// <summary>
	/// Parent node which contains a detector component.
	/// </summary>
	[Export] public DetectorParent DetectorParent;
	/// <summary>
	/// Parent node which contains a detectable component.
	/// </summary>
	[Export] public DetectableParent DetectableParent;
	/// <summary>
	/// Parent node which contains no components.
	/// </summary>
	[Export] public ComponentlessParent ComponentlessParent;
	/// <summary>
	/// Parent node which contains both a detector and a detectable component.
	/// </summary>
	[Export] public DetectorDetectableParent DetectorDetectableParent;

}
