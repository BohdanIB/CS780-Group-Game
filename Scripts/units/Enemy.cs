using Godot;
using System;

public partial class Enemy : PathFollower
{
	[Export] private string _enemyName = "jarold";
	
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
}
