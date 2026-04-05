using Godot;
using System;

/// <summary>
/// Defines animation sets that the entity has access to. WIP
/// </summary>
[GlobalClass]
public partial class AnimationPack : Resource
{
	[Export] public SpriteFrames Idle, Firing; // Todo: more definitions to come?

	public override string ToString()
	{
		return $"Idle: {Idle} - Firing: {Firing}";
	}

}
