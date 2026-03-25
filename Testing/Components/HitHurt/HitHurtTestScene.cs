using Godot;
using System;

public partial class HitHurtTestScene : Node2D
{
	[Export] public HitComponent HitComponent, HitComponent2;
	[Export] public HurtComponent HurtComponent, HurtComponent2;
}
