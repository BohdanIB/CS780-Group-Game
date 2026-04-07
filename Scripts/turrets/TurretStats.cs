
using Godot;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[GlobalClass]
public partial class TurretStats : Resource
{
	private const string TURRET_DIRECTORY_PATH = "res://Resources/Turret/";
	public enum Category
	{
		Ballista,
		Blade,
	};

	// Turret Stats
	[Export] public Category Type;
	[Export] public float AggroRadius;
	[Export] public float Health;
	[Export] public float FireRate; // shots per second, so (1/FireRate) will give you the time between shots for this turret.
	[Export] public ProjectileStats ProjectileStats; 
	[Export] public AnimationPack Animations;

	/// <summary>
	/// Get list of all turret stats.
	/// </summary>
	/// <returns></returns>
	public static List<TurretStats> LoadAllStats()
	{
		DirAccess directory = DirAccess.Open(TURRET_DIRECTORY_PATH);
		if (directory == null) return null;

		List<TurretStats> loadedTurrets = [];

		directory.ListDirBegin();

		foreach (var turretFileName in directory.GetFiles())
		{
			loadedTurrets.Add(ResourceLoader.Load<TurretStats>($"{TURRET_DIRECTORY_PATH}/{turretFileName}"));
		}

		directory.ListDirEnd();

		return loadedTurrets;
	}
	public override string ToString()
	{
		return $"{Type} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - Projectile: [{ProjectileStats}] - Animations: [{Animations}]";
	}


}
