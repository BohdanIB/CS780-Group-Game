
using Godot;

public partial class DetectableParent : Node2D
{
	[Export] public DetectableComponent Detectable;

	// public DetectableParent(){}
	// public DetectableParent(DetectableComponent detectable)
	// {
	// 	Detectable = detectable;
	// }

	// // Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// 	Debug.Assert(IsInstanceValid(this.DetectectableComponent));
	// }
}
