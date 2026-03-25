using Godot;
using System;

public partial class HitHurtTestScene : Node2D
{
	// [Export] public HitParent HitParent;
	// [Export] public HurtParent HurtParent;
	// [Export] public ComponentlessParent ComponentlessParent;
	[Export] public HitComponent HitComponent;
	[Export] public HurtComponent HurtComponent;
}
