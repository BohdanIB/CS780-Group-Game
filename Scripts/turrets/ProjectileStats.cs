
using Godot;
using System.Collections.Generic;

/// <summary>
/// TODO: 
/// Idea: Projectiles of different types can exist and potentially have different effects.
/// - When a projectile makes an impact, it may play through a set of frames from a sprite sheet, specific to its type.
/// - Projectiles can potentially be expanded to have different properties, or take some turret stat points (damage, etc.)
/// - ProjectileStats could dictate how projectiles move? (blade spins while moving towards enemy, bolt faces enemy, etc.)
/// </summary>
[GlobalClass]
public partial class ProjectileStats : Resource
{
	public static readonly List<ProjectileStats> ALL_PROJECTILES = LoadAllStats();
	private const string PROJECTILE_DIRECTORY_PATH = "res://Resources/Projectiles/";

	public enum Category
	{
		Bolt,
		Blade,
		Electro,
	};

	// Projectile Stats
	[Export] public Category Type;
	[Export] public float Speed;
	[Export] public float Damage;
	[Export] public float Hitbox;
	[Export] public AnimationPack Animations;
	[Export] public float AOERadius;
	[Export] public bool AOEFalloff;

	/// <summary>
	/// Get list of all projectile stats.
	/// </summary>
	/// <returns></returns>
	private static List<ProjectileStats> LoadAllStats()
	{
		DirAccess directory = DirAccess.Open(PROJECTILE_DIRECTORY_PATH);
		if (directory == null) return null;

		List<ProjectileStats> loadedProjectiles = [];

		directory.ListDirBegin();

		foreach (var projectileFileName in directory.GetFiles())
		{
			loadedProjectiles.Add(ResourceLoader.Load<ProjectileStats>($"{PROJECTILE_DIRECTORY_PATH}/{projectileFileName}"));
		}

		directory.ListDirEnd();

		return loadedProjectiles;
	}

	public override string ToString()
	{
		return $"{Type} - HitboxRadius: {Hitbox} - Speed: {Speed} - Damage: {Damage} - Animations: [{Animations}]";
	}
}
