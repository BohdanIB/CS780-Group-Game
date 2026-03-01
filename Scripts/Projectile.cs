using Godot;

/// <summary>
/// TODO: For now, Projectiles always will hit their target and will move towards the target until
/// </summary>
public partial class Projectile : Area2D
{
	[Export] private PathFollower _target;
	[Export] private float _damage = 10.0f;
	[Export] private float _speed = 100.0f;

	public void AssignTarget(PathFollower target, float damage = 10.0f, float speed = 100.0f)
	{
		_target = target;
		_damage = damage;
		_speed = speed;
	}

	public override void _Ready()
	{
		// OnProjectileCollision +=
		// When a Projectile collides with something, do damage to thing
		AreaEntered += (area) =>
		{
			if (area is PathFollower pf && pf == _target)
			{
				GD.Print($"Projectile hit target {pf.Name} for {_damage} damage");
				pf.ChangeHealth(_damage);
				QueueFree(); // todo: This might not be right
				// TODO: FREEING AND DISCONNECTION OF SIGNALS? https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
			}
		};
		GD.Print($"Projectile ready with stats: Damage - {_damage}, Speed: {_speed}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print($"Projectile stats: Damage - {_damage}, Speed: {_speed}");
		Position = Position.MoveToward(_target.Position, (float)delta * _speed);
	}

}
