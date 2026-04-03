
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[GlobalClass]
public partial class TurretStats : Resource
{
	public enum Category
	{
		Ballista,
		Blade,
	};
	public enum AnimatedSpriteFrameDirection
	{
		South, SouthWest, West, NorthWest, North, NorthEast, East, SouthEast,
	}
	private static int DirectionToFrameIndex(AnimatedSpriteFrameDirection dir) => dir switch
	{
		AnimatedSpriteFrameDirection.South     => 0,
		AnimatedSpriteFrameDirection.SouthWest => 1,
		AnimatedSpriteFrameDirection.West      => 2,
		AnimatedSpriteFrameDirection.NorthWest => 3,
		AnimatedSpriteFrameDirection.North     => 4,
		AnimatedSpriteFrameDirection.NorthEast => 5,
		AnimatedSpriteFrameDirection.East      => 6,
		AnimatedSpriteFrameDirection.SouthEast => 7,
		_ => throw new NotImplementedException()
	};
	
	/// <summary>
	/// Expecting rads from -PI to PI (where 0 corresponds to positive X, PI/2 corresponds to positive Y [WHICH IS DOWNWARD IN THE SCENE IN GODOT], etc.)
	/// </summary>
	/// <param name="rads"></param>
	/// <returns></returns>
	public static int RadsToFrameIndex(float rads) => Mathf.RadToDeg(rads) switch
	{
		< -157.5f => DirectionToFrameIndex(AnimatedSpriteFrameDirection.West),
		< -112.5f => DirectionToFrameIndex(AnimatedSpriteFrameDirection.NorthWest),
		< -67.5f  => DirectionToFrameIndex(AnimatedSpriteFrameDirection.North),
		< -22.5f  => DirectionToFrameIndex(AnimatedSpriteFrameDirection.NorthEast),
		< 22.5f   => DirectionToFrameIndex(AnimatedSpriteFrameDirection.East),
		< 67.5f   => DirectionToFrameIndex(AnimatedSpriteFrameDirection.SouthEast),
		< 112.5f  => DirectionToFrameIndex(AnimatedSpriteFrameDirection.South),
		< 157.5f  => DirectionToFrameIndex(AnimatedSpriteFrameDirection.SouthWest),
		<= 180f   => DirectionToFrameIndex(AnimatedSpriteFrameDirection.West),
		_ => throw new NotImplementedException(),
	};

	private record TurretBaseStats(float AggroRadius, float Health, float FireRate, ProjectileStats ProjectileStats, int SpriteFrame);
	private static readonly Dictionary<Category, TurretBaseStats> TURRET_BASE_STATS = new()
	{
		{Category.Ballista, 
			new TurretBaseStats(AggroRadius: 100f, Health: 100f, FireRate: 2f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 0)},
		{Category.Blade, 
			new TurretBaseStats(AggroRadius: 50f, Health: 100f, FireRate: 5f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Blade), SpriteFrame: 1)},
	};

	// Turret Stats
	private Category _type;
	private float _aggroRadius, _health, _fireRate, _projectileSpeed, _damage;
	private ProjectileStats _projectileStats;
	private int _spriteFrame;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float Health { get => _health; set => _health = value; }
	// FireRate represents shots per second, so (1/FireRate) will give you the time between shots for this turret.
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	public ProjectileStats ProjectileStats { get => _projectileStats; set => _projectileStats = value; } 
	public int SpriteFrame { get => _spriteFrame; set => _spriteFrame = value; }

	public TurretStats(Category type)
	{
		TurretBaseStats baseStats = TURRET_BASE_STATS[type];
		Type = type;
		AggroRadius = baseStats.AggroRadius;
		Health = baseStats.Health;
		FireRate = baseStats.FireRate;
		ProjectileStats = baseStats.ProjectileStats;
		SpriteFrame = baseStats.SpriteFrame;
	}

	public static TurretStats GetBaseTurretStats(Category type)
	{
		return new TurretStats(type);
	}

	public override string ToString()
	{
		return $"{_type} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - Projectile: [{ProjectileStats}]";
	}


}
