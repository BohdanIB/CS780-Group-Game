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
	// Fire rate 'per-second'
	private record TurretBaseStats(float AggroRadius, float Health, float FireRate, float ProjectileSpeed, float Damage, Vector2I AtlasCoordinates);
	private static readonly Dictionary<Category, TurretBaseStats> TURRET_BASE_STATS = new()
	{
		{Category.Balista, 
			new TurretBaseStats(AggroRadius: 100f, Health: 100f, FireRate: 2f, ProjectileSpeed: 100f, Damage: 10f, AtlasCoordinates: new Vector2I(0, 0))},
		{Category.Blade, 
			new TurretBaseStats(AggroRadius: 50f, Health: 100f, FireRate: 5f, ProjectileSpeed: 100f, Damage: 20f, AtlasCoordinates: new Vector2I(1, 0))},
	};

	private Category _type;
	public Category Type { get => _type; set => _type = value; }

	private float _aggroRadius, _health, _fireRate, _projectileSpeed, _damage;
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float Health { get => _health; set => _health = value; }
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	public float ProjectileSpeed { get => _projectileSpeed; set => _projectileSpeed = value; }
	public float Damage { get => _damage; set => _damage = value; }

	private Vector2I _atlasCoordinates;
	public Vector2I AtlasCoordinates { get => _atlasCoordinates; set => _atlasCoordinates = value; }

	public TurretStats(Category type)
	{
		Type = type;
		TurretBaseStats baseTurret = TURRET_BASE_STATS[Type];
		AggroRadius = baseTurret.AggroRadius;
		Health = baseTurret.Health;
		FireRate = baseTurret.FireRate;
		ProjectileSpeed = baseTurret.ProjectileSpeed;
		Damage = baseTurret.Damage;
		AtlasCoordinates = baseTurret.AtlasCoordinates;
	}

	public override string ToString()
	{
		return $"Type: {nameof(_type)} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - ProjectileSpeed: {ProjectileSpeed} - Damage: {Damage}";
	}


}
