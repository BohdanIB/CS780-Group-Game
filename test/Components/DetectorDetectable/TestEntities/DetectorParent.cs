
using Godot;

public partial class DetectorParent : Node2D
{
	[Export] public DetectorComponent Detector;

	// public DetectorParent(){}
	// public DetectorParent(DetectorComponent detector)
	// {
	// 	Detector = detector;
	// }

	// // Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// 	Debug.Assert(IsInstanceValid(this.DetectorComponent));
	// }

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
}
