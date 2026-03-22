using Godot;
using System;
using System.Diagnostics;

public partial class DetectableParent : Node2D
{
	[Export] public DetectableComponent DetectectableComponent {get;set;}

	// // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(IsInstanceValid(this.DetectectableComponent));
	}
}
