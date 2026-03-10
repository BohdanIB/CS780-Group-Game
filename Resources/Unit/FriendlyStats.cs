
using Godot;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public partial class FriendlyStats : Resource
{
	public enum Category
	{
		Regular,
		Loaded, // todo
	};

	private record BaseStats(float HitboxRadius, float AggroRadius, float Health, float FireRate, float MovementSpeed, ProjectileStats ProjectileStats, int SpriteFrame);
	private static readonly Dictionary<Category, BaseStats> BASE_STATS = new()
	{
		{Category.Regular, 
			new BaseStats(HitboxRadius: 5f, AggroRadius: 25f, Health: 50, FireRate: 2f, MovementSpeed: 10f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 0)},
		{Category.Loaded, 
			new BaseStats(HitboxRadius: 5f, AggroRadius: 10f, Health: 50f, FireRate: 2f, MovementSpeed: 5f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 1)},
	};

	// Friendly Stats
	[Export] private Category _type;
	[Export] private float _hitboxRadius, _aggroRadius, _health, _fireRate, _movementSpeed;
	[Export] private ProjectileStats _projectileStats;
	[Export] private int _spriteFrame;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float HitboxRadius { get => _hitboxRadius; set => _hitboxRadius = value; }
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float Health { get => _health; set => _health = value; }
	// Shots per second, so (1/FireRate) will give you the time between shots for this enemy.
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	// Pixels per second.
	public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
	public ProjectileStats ProjectileStats { get => _projectileStats; set => _projectileStats = value; } 
	public int SpriteFrame { get => _spriteFrame; set => _spriteFrame = value; }

	public FriendlyStats(Category type)
	{
		BaseStats baseStats = BASE_STATS[type];
		Type = type;
		HitboxRadius = baseStats.HitboxRadius;
		AggroRadius = baseStats.AggroRadius;
		Health = baseStats.Health;
		FireRate = baseStats.FireRate;
		MovementSpeed = baseStats.MovementSpeed;
		ProjectileStats = baseStats.ProjectileStats;
		SpriteFrame = baseStats.SpriteFrame;
	}

	public static FriendlyStats GetBaseFriendlyStats(Category type)
	{
		return new FriendlyStats(type);
	}
	public override string ToString()
	{
		return $"{Type} - HitboxRadius: {HitboxRadius} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - MovementSpeed: {MovementSpeed} - Projectile: [{ProjectileStats}]";
	}

}
