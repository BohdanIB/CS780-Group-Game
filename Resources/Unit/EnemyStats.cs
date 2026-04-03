
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public partial class EnemyStats : Resource
{
	public enum Category
	{
		Regular,
		Strong, // Creative, I know. Truly my genius knows no bounds
	};
	public enum AnimatedSpriteFrameDirection
	{
		SouthWest, NorthWest, NorthEast, SouthEast,
	}
	private static int DirectionToFrameIndex(AnimatedSpriteFrameDirection dir) => dir switch
	{
		AnimatedSpriteFrameDirection.SouthEast => 0,
		AnimatedSpriteFrameDirection.SouthWest => 1,
		AnimatedSpriteFrameDirection.NorthWest => 2,
		AnimatedSpriteFrameDirection.NorthEast => 3,
		_ => throw new NotImplementedException()
	};
	/// <summary>
	/// Expecting rads from -PI to PI (where 0 corresponds to positive X, PI/2 corresponds to positive Y [WHICH IS DOWNWARD IN THE SCENE IN GODOT], etc.)
	/// </summary>
	/// <param name="rads"></param>
	/// <returns></returns>
	public static int RadsToFrameIndex(float rads) => Mathf.RadToDeg(rads) switch
	{
		< -90f   => DirectionToFrameIndex(AnimatedSpriteFrameDirection.NorthWest),
		< 0f     => DirectionToFrameIndex(AnimatedSpriteFrameDirection.NorthEast),
		< 90f    => DirectionToFrameIndex(AnimatedSpriteFrameDirection.SouthEast),
		< 180f  => DirectionToFrameIndex(AnimatedSpriteFrameDirection.SouthWest),
		_ => throw new NotImplementedException(),
	};

	private record BaseStats(float HitboxRadius, float AggroRadius, float Health, float FireRate, float MovementSpeed, ProjectileStats ProjectileStats, int SpriteFrame);
	private static readonly Dictionary<Category, BaseStats> BASE_STATS = new()
	{
		{Category.Regular, 
			new BaseStats(HitboxRadius: 5f, AggroRadius: 50f, Health: 50, FireRate: 4f, MovementSpeed: 15f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 0)},
		{Category.Strong, 
			new BaseStats(HitboxRadius: 5f, AggroRadius: 25f, Health: 100f, FireRate: 4f, MovementSpeed: 10f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 1)},
	};

	// Enemy Stats
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

	public EnemyStats(Category type)
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

	public static EnemyStats GetBaseEnemyStats(Category type)
	{
		return new EnemyStats(type);
	}
	public override string ToString()
	{
		return $"{Type} - HitboxRadius: {HitboxRadius} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - MovementSpeed: {MovementSpeed} - Projectile: [{ProjectileStats}]";
	}

}
