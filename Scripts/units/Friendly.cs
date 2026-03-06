
using Godot;

public partial class Friendly : GenericPathFollower
{
	private FriendlyStats _stats;

	/// <summary>
	/// Initializes friendly with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(FriendlyStats.Category type)
	{
		Initialize(new FriendlyStats(type));
	}
	/// <summary>
	/// Initializes friendly with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(FriendlyStats stats)
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
		// Todo
	}

	public void UpdateStats(FriendlyStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		UpdateFriendlyHitboxRadius();
		UpdateFriendlyAggroRadius();
		UpdateFriendlySprite();

		// Todo: Add more updates

		UpdateFriendlyHealth();
	}
	private void UpdateFriendlyHitboxRadius()
	{
		((CircleShape2D)_hitboxCollisionShape2D.Shape).Radius = _stats.HitboxRadius; // TODO: Better way of doing this?
	}
	private void UpdateFriendlyAggroRadius()
	{
		((CircleShape2D)_aggroCollisionShape2D.Shape).Radius = _stats.AggroRadius; // TODO: Better way of doing this?
	}
	private void UpdateFriendlySprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	private void UpdateFriendlyHealth()
	{
		_health = _stats.Health;
	}
	public override string ToString()
	{
		return $"Friendly '{Name}': {_stats}";
	}
}
