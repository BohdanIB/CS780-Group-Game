using Godot;
using System;
using System.Diagnostics;

public partial class DetectorParent : Node2D
{
	[Export] public DetectorComponent DetectorComponent {get;set;}

	// // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(IsInstanceValid(this.DetectorComponent));
	}

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
}
