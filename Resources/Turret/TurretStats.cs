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
		Balista,
		Blade,
	};
	private record TurretBaseStats(float AggroRadius, float Health, float FireRate, ProjectileStats ProjectileStats, Vector2I AtlasCoordinates);
	private static readonly Dictionary<Category, TurretBaseStats> TURRET_BASE_STATS = new()
	{
		{Category.Balista, 
			new TurretBaseStats(AggroRadius: 100f, Health: 100f, FireRate: 2f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Bolt), AtlasCoordinates: new Vector2I(0, 0))},
		{Category.Blade, 
			new TurretBaseStats(AggroRadius: 50f, Health: 100f, FireRate: 5f, ProjectileStats: new ProjectileStats(ProjectileStats.Category.Blade), AtlasCoordinates: new Vector2I(1, 0))},
	};

	// Turret Stats
	private Category _type;
	private float _aggroRadius, _health, _fireRate, _projectileSpeed, _damage;
	private ProjectileStats _projectileStats;
	private Vector2I _atlasCoordinates;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float Health { get => _health; set => _health = value; }
	// FireRate represents shots per second, so (1/FireRate) will give you the time between shots for this turret.
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	public ProjectileStats ProjectileStats { get => _projectileStats; set => _projectileStats = value; } 
	public Vector2I AtlasCoordinates { get => _atlasCoordinates; set => _atlasCoordinates = value; }

	public TurretStats(Category type)
	{
		Type = type;
		TurretBaseStats baseStats = TURRET_BASE_STATS[Type];
		AggroRadius = baseStats.AggroRadius;
		Health = baseStats.Health;
		FireRate = baseStats.FireRate;
		ProjectileStats = baseStats.ProjectileStats;
		AtlasCoordinates = baseStats.AtlasCoordinates;
	}

	public override string ToString()
	{
		return $"{_type} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - Projectile: [{ProjectileStats}]";
	}


}
