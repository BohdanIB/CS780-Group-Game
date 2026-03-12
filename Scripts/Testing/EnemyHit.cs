using Godot;
using System;

public partial class EnemyHit : Node2D
{
	[Export] private Enemy _enemy;
	[Export] private HitComponent _hitComponent;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		const float MOVEMENT_SPEED = 200f;
		// _hitComponent.Position.MoveToward(Vector2.Left, (float)delta * 200f);
		if (IsInstanceValid(_hitComponent))
		{
			_hitComponent.Position += new Vector2(-MOVEMENT_SPEED * (float)delta, 0);
		}
	}
}
