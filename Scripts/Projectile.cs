using Godot;

/// <summary>
/// TODO: For now, Projectiles always will hit their target and will move towards the target until
/// </summary>
public partial class Projectile : Area2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	[Signal]
	public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats);

	[Export] private ProjectileStats.Category _projectileType;
	private ProjectileStats _stats;
	private PathFollower _target;
	private Vector2 _targetLastKnownLocation;

	/// <summary>
	/// Initializes projectile with custom stats.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(PathFollower target, ProjectileStats projectileStats)
	{
		_target = target;
		if (_target == null) // Todo: Might not be proper before _Ready method?
		{
			GD.Print($"Projectile was instantiated, but target no longer exists... Freeing projectile.");
			QueueFree();
			return;
		}
		_targetLastKnownLocation = _target.Position;
		_stats = projectileStats;
	}
	/// <summary>
	/// Initializes projectile with "generic" base stats for given type.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="projectileType"></param>
	public void Initialize(PathFollower target, ProjectileStats.Category projectileType)
	{
		Initialize(target, new ProjectileStats(projectileType));
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is PathFollower pf && pf == _target)
			{
				ProjectileImpact();
			}
		};

		// GD.Print($"Projectile ready with stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print($"Projectile stats: {_stats}");
		// TODO: WIP This does not solve problem where pathfollower dies before bullet reaches...
		if (_target != null)
		{
			_targetLastKnownLocation = _target.Position;
		}

		Position = Position.MoveToward(_targetLastKnownLocation, (float)delta * _stats.Speed);
		if (Position.DistanceTo(_targetLastKnownLocation) < MIN_TARGET_DISTANCE)
		{
			// todo: Target ended up dying before projectile could reach it.
			if (_target == null)
			{
				GD.Print($"\tProjectile reached target's last known location without colliding with target.");
				ProjectileImpact();
			}
			else
			{
				GD.Print($"\tProjectile reached target's last known location without colliding with target AND THE TARGET STILL EXISTS. Performing normal hit on target.");
				ProjectileImpact();
			}
		}
	}

	private void ProjectileImpact()
	{
		// Todo: WIP - Potentially add animation or some other effects to projectile on impact? May want to incorporate signal somehow.
		EmitSignal(SignalName.OnProjectileImpact, Position, _stats);
		if (_target != null)
		{
			GD.Print($"Projectile hit target {_target.Name} for {_stats.Damage} damage");
			_target.ChangeHealth(_stats.Damage);
		}
		QueueFree();
		// TODO: FREEING AND DISCONNECTION OF SIGNALS? https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
	}

}
