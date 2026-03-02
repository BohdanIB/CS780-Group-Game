using Godot;
using System.Collections.Generic;

/// <summary>
/// TODO: 
/// Idea: Projectiles of different types can exist and potentially have different effects.
/// - When a projectile makes an impact, it may play through a set of frames from a sprite sheet, specific to its type.
/// - Projectiles can potentially be expanded to have different properties, or take some turret stat points (damage, etc.)
/// </summary>
[GlobalClass]
public partial class ProjectileStats : Resource
{
	public enum Category
	{
		Bolt,
		Blade,
	};
	private record ProjectileBaseStats(float Speed, float Damage, Vector2I AtlasCoordinates);
	private static readonly Dictionary<Category, ProjectileBaseStats> PROJECTILE_BASE_STATS = new()
	{
		{Category.Bolt, 
			new ProjectileBaseStats(Speed: 100f, Damage: 10f, AtlasCoordinates: new Vector2I(0, 0))},
		{Category.Blade, 
			new ProjectileBaseStats(Speed: 100f, Damage: 20f, AtlasCoordinates: new Vector2I(1, 0))},
	};

	// Projectile Stats
	private Category _type;
	private float _speed, _damage;
	private Vector2I _atlasCoordinates;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float Speed { get => _speed; set => _speed = value; }
	public float Damage { get => _damage; set => _damage = value; }
	public Vector2I AtlasCoordinates { get => _atlasCoordinates; set => _atlasCoordinates = value; }

	public ProjectileStats(Category type)
	{
		Type = type;
		ProjectileBaseStats baseStats = PROJECTILE_BASE_STATS[Type];
		Speed = baseStats.Speed;
		Damage = baseStats.Damage;
		AtlasCoordinates = baseStats.AtlasCoordinates;
	}

	public override string ToString()
	{
		return $"{_type} - Speed: {Speed} - Damage: {Damage}";
	}
}