using Godot;
using System.Collections.Generic;

/// <summary>
/// 
// - Type
// - Atlas coords
// - Stats
//   - Radius
//   - Fire rate
//   - Damage
//   - Other?
/// </summary>
[GlobalClass]
public partial class TurretStats : Resource
{
	// Fire rate is 'per-second'
	public record TurretBaseStats(float AggroRadius, float Health, float FireRate, float Damage, Vector2I AtlasCoordinates);

	public enum Category
	{
		Balista,
		Blade,
	};
	private static readonly Dictionary<Category, TurretBaseStats> TURRET_BASE_STATS = new()
	{
		{Category.Balista, 
			new TurretBaseStats(AggroRadius: 100f, Health: 100f, FireRate: 2f, Damage: 10f, AtlasCoordinates: new Vector2I(0, 0))},
		{Category.Blade, 
			new TurretBaseStats(AggroRadius: 50f, Health: 100f, FireRate: 5f, Damage: 20f, AtlasCoordinates: new Vector2I(1, 0))},
	};

	private Category _type;
	public Category Type { get => _type; set => _type = value; }

	private float _aggroRadius, _health, _fireRate, _damage;
	public float AggroRadius { get => _aggroRadius; set => _aggroRadius = value; }
	public float Health { get => _health; set => _health = value; }
	public float FireRate { get => _fireRate; set => _fireRate = value; }
	public float Damage { get => _damage; set => _damage = value; }

	private Vector2I _atlasCoordinates;
	// public Vector2I AtlasCoordinates { get => GetAtlasCoordinates(); }
	public Vector2I AtlasCoordinates { get => _atlasCoordinates; set => _atlasCoordinates = value; }

	public TurretStats(Category type)
	{
		Type = type;
		TurretBaseStats baseTurret = TURRET_BASE_STATS[Type];
		// CurrentStats = new(
		// 	AggroRadius: currTurretRecord.AggroRadius,
		// 	Health: currTurretRecord.Health,
		// 	FireRate: currTurretRecord.FireRate,
		// 	Damage: currTurretRecord.Damage,
		// 	AtlasCoordinates: currTurretRecord.AtlasCoordinates
		// );
		AggroRadius = baseTurret.AggroRadius;
		Health = baseTurret.Health;
		FireRate = baseTurret.FireRate;
		Damage = baseTurret.Damage;
		AtlasCoordinates = baseTurret.AtlasCoordinates;
	}

	// public static Vector2I GetAtlasCoordinates(Category type)
	// {
	// 	return TURRET_ATLAS_COORDS[type];
	// }
	// public Vector2I GetAtlasCoordinates()
	// {
	// 	return GetAtlasCoordinates(Type);
	// }

}
