using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class TurretStats : Resource
{
	private const string TURRET_DIRECTORY_PATH = "res://Resources/Turret/";

	public enum Category
	{
		Ballista,
		Blade
	}


	[Export] public Category Type;
	[Export] public float AggroRadius;
	[Export] public float Health;
	[Export] public float FireRate; 
	[Export] public ProjectileStats ProjectileStats;
	[Export] public AnimationPack Animations;

	// Your cost system (needed by TurretPlacer)
	[Export] public int Cost;

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
		return $"{Type} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - Cost: {Cost}";
	}
}
