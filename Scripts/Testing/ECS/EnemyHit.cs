using Godot;
using System;

public partial class EnemyHit : Node2D
{
	[Export] private Enemy _enemy;
	[Export] private Projectile 
	_projectileTargeted,     // Specifically targets enemy (should hit enemy)
	_projectileTargetScenes, // No target, Enemy is part of hitableScenes (should hit enemy)
	_projectileNone,         // No target, no hitableScenes (should not hit enemy)
	_projectileKiller;       // Targets enemy, kills enemy

	// public override void _Ready()
	// {
	// }

	// public override void _Process(double delta)
	// {
	// }
}
