using Godot;
using System;

public partial class EnemyHit : Node2D
{
	private Enemy _enemy;
	private Projectile _projectile;

	public override void _Ready()
	{
		_enemy = GetNode<Enemy>("Enemy");
		_projectile = GetNode<Projectile>("Projectile");
	}

	public override void _Process(double delta)
	{
		// const float MOVEMENT_SPEED = 200f;
		// if (IsInstanceValid(_projectile))
		// {
		// 	_projectile.Position += new Vector2(-MOVEMENT_SPEED * (float)delta, 0);
		// }
	}
}
