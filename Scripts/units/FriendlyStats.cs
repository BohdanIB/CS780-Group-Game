
using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class FriendlyStats : Resource
{
	private const string FRIENDLY_DIRECTORY_PATH = "res://Resources/Unit/Friendly/";
	public enum Category
	{
		Regular,
		Loaded, // todo
	};

	// Friendly Stats
	[Export] public Category Type;
	[Export] public float HitboxRadius;
	[Export] public float AggroRadius;
	[Export] public float Health;
	[Export] public float FireRate; // Shots per second, so (1/FireRate) will give you the time between shots.
	[Export] public float MovementSpeed; // pixels per second
	[Export] public ProjectileStats ProjectileStats; 
	[Export] public AnimationPack Animations;

	/// <summary>
	/// Get list of all friendly stats.
	/// </summary>
	/// <returns></returns>
	public static List<FriendlyStats> LoadAllStats()
	{
		DirAccess directory = DirAccess.Open(FRIENDLY_DIRECTORY_PATH);
		if (directory == null) return null;

		List<FriendlyStats> loadedFriendlies = [];

		directory.ListDirBegin();

		foreach (var friendlyFileName in directory.GetFiles())
		{
			loadedFriendlies.Add(ResourceLoader.Load<FriendlyStats>($"{FRIENDLY_DIRECTORY_PATH}/{friendlyFileName}"));
		}

		directory.ListDirEnd();

		return loadedFriendlies;
	}
	public override string ToString()
	{
		return $"{Type} - HitboxRadius: {HitboxRadius} - AggroRadius: {AggroRadius} - Health: {Health} - FireRate: {FireRate} - MovementSpeed: {MovementSpeed} - Projectile: [{ProjectileStats}] - Animations: [{Animations}]";
	}

}
