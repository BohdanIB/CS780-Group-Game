using Godot;
using System.Collections.Generic;

[GlobalClass]


/// <summary>
/// 
/// </summary>
[GlobalClass]
public partial class TurretStats : StructureStats
{
	public static readonly List<TurretStats> ALL_TURRETS = LoadAllStats();
	private const string TURRET_DIRECTORY_PATH = "res://Resources/Turret/";

	public enum Category
	{
		Ballista,
		Blade
	}


	// Turret Stats
	[Export] public Category Type;
	[Export] public float AggroRadius, DetectableRadius, HitboxRadius;
	[Export] public float Health;
	[Export] public float FireRate; // shots per second, so (1/FireRate) will give you the time between shots for this turret.
	[Export] public ProjectileStats ProjectileStats;
	[Export] public AnimationPack Animations;

	[Export] public int Cost;


	/// <summary>
	/// Get list of all turret stats.
	/// </summary>
	/// <returns></returns>
	public static List<TurretStats> LoadAllStats()
	{
		DirAccess directory = DirAccess.Open(TURRET_DIRECTORY_PATH);
		if (directory == null) return null;

		List<TurretStats> loadedTurrets = new();

		directory.ListDirBegin();

		foreach (var turretFileName in directory.GetFiles())
		{
			if (turretFileName.EndsWith(".tres"))
			{
				loadedTurrets.Add(ResourceLoader.Load<TurretStats>($"{TURRET_DIRECTORY_PATH}/{turretFileName}"));
			}
		}

		directory.ListDirEnd();

		return loadedTurrets;
	}

	public override string ToString()
	{
		return $"{Type} - AggroRadius: {AggroRadius} - DetectableRadius: {DetectableRadius} - HitboxRadius: {HitboxRadius} - Health: {Health} - FireRate: {FireRate} - Cost: {Cost}";
	}
}
