
using Godot;

/// <summary>
/// TODO: Merge functionality between turrets and enemies somehow (shooter component which gets plugged into enemy and turret?)
///   - Shares a lot of functions with turret honestly...
/// </summary>
public partial class Enemy : GenericPathFollower
{
	// Scene Children
	[Export] private Timer _shotCooldownTimer;

	// Preloaded Scenes
	[Export] private PackedScene _projectileScene;

	private EnemyStats _stats;

	/// <summary>
	/// Initializes enemy with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(EnemyStats.Category type)
	{
		Initialize(new EnemyStats(type));
	}
	/// <summary>
	/// Initializes enemy with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(EnemyStats stats)
	{
		UpdateStats(stats);
	}

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		// Todo: Shoot at friendlies in range. Components?
	}

	public void UpdateStats(EnemyStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		UpdateEnemyHitboxRadius();
		UpdateEnemyAggroRadius();
		UpdateEnemySprite();

		// Todo: Add more updates

		UpdateEnemyHealth();
	}
	private void UpdateEnemyHitboxRadius()
	{
		((CircleShape2D)_hitboxCollisionShape2D.Shape).Radius = _stats.HitboxRadius; // TODO: Better way of doing this?
	}
	private void UpdateEnemyAggroRadius()
	{
		((CircleShape2D)_aggroCollisionShape2D.Shape).Radius = _stats.AggroRadius; // TODO: Better way of doing this?
	}
	private void UpdateEnemySprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	private void UpdateEnemyHealth()
	{
		_health = _stats.Health;
	}
	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
