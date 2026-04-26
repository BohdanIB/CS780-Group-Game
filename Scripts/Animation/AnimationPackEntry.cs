using Godot;
using System;

/// <summary>
/// Base resource for SpriteFrames for AnimationPacks. Typing is used to tell AnimationComponent 
/// what state it should be in for animating sprites.
/// </summary>
[GlobalClass]
public partial class AnimationPackEntry : Resource
{
	public enum State
	{
		Idle,
		Bob,
		Firing,
		// etc.
	}
    [Export] public State EntryState;
    [Export] public SpriteFrames Frames;
}
