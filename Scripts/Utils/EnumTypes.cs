
using System;
using System.Collections.Generic;
using Godot;

namespace CS780GroupProject.Scripts.Utils
{
	/// <summary>
	/// Groups that exist in the game. Used for determining if certain components can interact with other components.
	/// <br/><br/>
	/// Example: A HitComponent may need to understand what types of entities it can generically hit rather than a specific target.
	/// <br/><br/>
	/// WARNING: Changes to this enum can result in displacement of preset EntityGroups within scenes.
	/// </summary>
	// [Flags] public enum EntityGroups
	// {
	// 	None      = 0,
	// 	Enemy     = 1 << 0,
	// 	Friendly  = 1 << 1,
	// 	Structure = 1 << 2,
	// 	Tower     = 1 << 3,
	// }

	// public List<Type> GetEntityGroupTypes(EntityGroups entityGroups)
	// {
	// 	foreach (var a in entityGroups)
	// 	{
			
	// 	}


	// 	if ((entityGroups & EntityGroups.Enemy) != EntityGroups.None)
	// 	{
	// 		return typeof(Enemy);
	// 	}
	// 	if ((entityGroups & EntityGroups.Friendly) != EntityGroups.None)
	// 	{
	// 		return typeof(Friendly);
	// 	}
	// }

	/// <summary>
	/// Stolen from BloonsTD! Targeting priorities for entities that can shoot or need to choose entities within influence.
	/// </summary>
	public enum TargetingMode
	{
		Random, // Random
		First,  // Furthest down path
		Last,   // Closest to spawn
		Close,  // Closest to tower
		Weak,   // Weakest enemies
		Strong, // Strongest enemies
	}
}
