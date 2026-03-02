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
	private record ProjectileBaseStats(float Speed, float Damage, int SpriteFrame);
	private static readonly Dictionary<Category, ProjectileBaseStats> PROJECTILE_BASE_STATS = new()
	{
		{Category.Bolt, 
			new ProjectileBaseStats(Speed: 100f, Damage: 10f, SpriteFrame: 0)},
		{Category.Blade, 
			new ProjectileBaseStats(Speed: 100f, Damage: 20f, SpriteFrame: 1)},
	};

	// Projectile Stats
	private Category _type;
	private float _speed, _damage;
	private int _spriteFrame;

	// Getters + Setters
	public Category Type { get => _type; set => _type = value; }
	public float Speed { get => _speed; set => _speed = value; }
	public float Damage { get => _damage; set => _damage = value; }
	public int SpriteFrame { get => _spriteFrame; set => _spriteFrame = value; }

	public ProjectileStats(Category type)
	{
		Type = type;
		ProjectileBaseStats baseStats = PROJECTILE_BASE_STATS[Type];
		Speed = baseStats.Speed;
		Damage = baseStats.Damage;
		SpriteFrame = baseStats.SpriteFrame;
	}

	public override string ToString()
	{
		return $"{_type} - Speed: {Speed} - Damage: {Damage}";
	}
}