
using Godot;
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
	private record TurretBaseStats(float HitboxRadius, float AggroRadius, float DetectableRadius, float Health, float FireRate, ProjectileStats ProjectileStats, int SpriteFrame);
	private static readonly Dictionary<Category, TurretBaseStats> TURRET_BASE_STATS = new()
	{
		{Category.Ballista, 
			new TurretBaseStats(HitboxRadius: 16f, AggroRadius: 100f, DetectableRadius: 16f, Health: 100f, FireRate: 2f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), SpriteFrame: 0)},
		{Category.Blade, 
			new TurretBaseStats(HitboxRadius: 16f, AggroRadius: 50f, DetectableRadius: 16f, Health: 100f, FireRate: 5f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Blade), SpriteFrame: 1)},
	};

	// Turret Stats
	[Export] private Category _type;
	[Export] private float _hitboxRadius, _aggroRadius, _detectableRadius, _health, _fireRate;
	[Export] private ProjectileStats _projectileStats;
	[Export] private int _spriteFrame;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float HitboxRadius { get => _hitboxRadius; set => _hitboxRadius = value; }
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float DetectableRadius { get => _detectableRadius; set => _detectableRadius = value; }
	public float Health { get => _health; set => _health = value; }
	// FireRate represents shots per second, so (1/FireRate) will give you the time between shots for this turret.
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	public ProjectileStats ProjectileStats { get => _projectileStats; set => _projectileStats = value; } 
	public int SpriteFrame { get => _spriteFrame; set => _spriteFrame = value; }

	public TurretStats(Category type)
	{
		TurretBaseStats baseStats = TURRET_BASE_STATS[type];
		Type = type;
		HitboxRadius = baseStats.HitboxRadius;
		AggroRadius = baseStats.AggroRadius;
		DetectableRadius = baseStats.DetectableRadius;
		Health = baseStats.Health;
		FireRate = baseStats.FireRate;
		ProjectileStats = baseStats.ProjectileStats;
		SpriteFrame = baseStats.SpriteFrame;
	}
	public TurretStats()
	{
		TurretBaseStats baseStats = TURRET_BASE_STATS[Type];
		HitboxRadius = baseStats.HitboxRadius;
		AggroRadius = baseStats.AggroRadius;
		DetectableRadius = baseStats.DetectableRadius;
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
		return $"{Type} - HitboxRadius: {HitboxRadius} - AggroRadius: {AggroRadius} - DetectableRadius: {DetectableRadius} - Health: {Health} - FireRate: {FireRate} - Projectile: [{ProjectileStats}]";
	}

}
