
namespace CS780GroupProject.Scripts.Utils
{
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
