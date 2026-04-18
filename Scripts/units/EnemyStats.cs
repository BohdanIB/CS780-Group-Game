
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[GlobalClass]
public partial class EnemyStats : Resource
{
	public static readonly List<EnemyStats> ALL_ENEMIES = LoadAllStats();
	private const string ENEMY_DIRECTORY_PATH = "res://Resources/Unit/Enemy/";
	public enum Category
	{
		Regular,
		Strong, // Creative, I know. Truly my genius knows no bounds
	};

	// Enemy Stats
	[Export] public Category Type;
	[Export] public float AggroRadius, DetectableRadius, HitboxRadius;
	[Export] public float Health;
	
	[Export] public float FireRate; // Shots per second, so (1/FireRate) will give you the time between shote.
	[Export] public float MovementSpeed; // pixels per second
	[Export] public ProjectileStats ProjectileStats; 
	[Export] public AnimationPack Animations;

	/// <summary>
	/// Get list of all enemy stats.
	/// </summary>
	/// <returns></returns>
	private static List<EnemyStats> LoadAllStats()
	{
		DirAccess directory = DirAccess.Open(ENEMY_DIRECTORY_PATH);
		if (directory == null) return null;

		List<EnemyStats> loadedEnemies = [];

		directory.ListDirBegin();

		foreach (var enemyFileName in directory.GetFiles())
		{
			loadedEnemies.Add(ResourceLoader.Load<EnemyStats>($"{ENEMY_DIRECTORY_PATH}/{enemyFileName}"));
		}

		directory.ListDirEnd();

		return loadedEnemies;
	}
	public override string ToString()
	{
		return $"{Type} - AggroRadius: {AggroRadius} - DetectableRadius: {DetectableRadius} - HitboxRadius: {HitboxRadius} - Health: {Health} - FireRate: {FireRate} - MovementSpeed: {MovementSpeed} - Projectile: [{ProjectileStats}] - Animations: [{Animations}]";
	}

}
