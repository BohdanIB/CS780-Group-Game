using Godot;
using System;


namespace CS780GroupProject.Scripts.Utils
{
	
	// [Flags] public enum GroupTypes
	// {
	// 	None      = 0,
	// 	Enemy     = 1 << 0,
	// 	Friendly  = 1 << 1,
	// 	Structure = 1 << 2,
	// 	Tower     = 1 << 3,
	// }
	// public partial class Globals
	// {

	// }

	public partial class Groups : Node
	{
		const string TURRET = "turret";
		const string STRUCTURE = "structure";
		const string PROJECTILE = "projectile";
		const string FRIENDLY = "friendly";
		const string ENEMY = "enemy";

	
		/// <summary>
		/// Groups that exist in the game. Used for determining if certain components can interact with other components.
		/// <br/><br/>
		/// Example: A HitComponent may need to understand what types of entities it can generically hit rather than a specific target.
		/// <br/><br/>
		/// WARNING: Changes to this enum can result in displacement of preset GroupTypes within scenes.
		/// </summary>
		[Flags] public enum GroupTypes
		{
			None = 0,
			Turret = 1 << 0,
			Structure = 1 << 1,
			Projectile = 1 << 2,
			Friendly = 1 << 3,
			Enemy = 1 << 4,
		}

		const string CAT = "🐱‍🏍";
	}
}
