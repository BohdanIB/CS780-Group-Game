using Godot;
using System;
using System.Collections.Generic;

public partial class AnimationComponent : AnimatedSprite2D
{
	private enum Direction
	{
		North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest,
	}

	[Export] public AnimationPack Animations;

	/// <summary>
	/// Initialize AnimationComponent with direction.
	/// </summary>
	/// <param name="animations"></param>
	/// <param name="directionRads"></param>
	/// <param name="state"></param>
	public void Initialize(AnimationPack animations, float directionRads, AnimationPackEntry.State state = AnimationPackEntry.State.Idle)
	{
		Initialize(animations, state);
		SetDirection(directionRads);
	}
	/// <summary>
	/// Initialize AnimationComponent without direction.
	/// </summary>
	/// <param name="animations"></param>
	/// <param name="state"></param>
	public void Initialize(AnimationPack animations, AnimationPackEntry.State state = AnimationPackEntry.State.Idle)
	{
		Animations = animations;
		SetState(state);
	}

	/// <summary>
	/// Set state of animation component. If the state exists in the current AnimationPack, then set and play state.
	/// </br>
	/// Direction determines animation set to play within selected SpriteFrames.
	/// </summary>
	/// <param name="state"></param>
	/// <param name="direction"></param>
	public void SetState(AnimationPackEntry.State state)
	{
		foreach (var entry in Animations.Animations)
		{
			if (entry.EntryState == state)
			{
				this.SpriteFrames = entry.Frames;
				if (SpriteFrames.GetAnimationNames() is var animationNames && animationNames != null && animationNames.Length > 0)
				{
					Play(animationNames[0]); // start playing some animation within spriteframes state
				}
				break;
			}
		}
	}

	/// <summary>
	/// Given a point for this animation to look at from current
	/// </summary>
	/// <param name="directionRads"></param>
	public void SetDirection(float directionRads)
	{
	// 	// from.AngleToPoint(to)
		var dir = RadsToDirection(directionRads);
		var animationName = DirectionToAnimationName(dir);
		if (SpriteFrames.HasAnimation(animationName))
		{
			Play(animationName);
		}
	}

	private static Direction RadsToDirection(float rads)
	{
		return DegreesToDirection(Mathf.RadToDeg(rads));
	}
	private static Direction DegreesToDirection(float degrees) => degrees switch
	{
		< -157.5f => Direction.West,
		< -112.5f => Direction.NorthWest,
		< -67.5f  => Direction.North,
		< -22.5f  => Direction.NorthEast,
		< 22.5f   => Direction.East,
		< 67.5f   => Direction.SouthEast,
		< 112.5f  => Direction.South,
		< 157.5f  => Direction.SouthWest,
		<= 180f   => Direction.West,
		_ => throw new NotImplementedException(),
	};
	private static string DirectionToAnimationName(Direction dir) => dir switch
	{
		Direction.North     => "north",
		Direction.NorthEast => "north_east",
		Direction.East      => "east",
		Direction.SouthEast => "south_east",
		Direction.South     => "south",
		Direction.SouthWest => "south_west",
		Direction.West      => "west",
		Direction.NorthWest => "north_west",
		_ => throw new NotImplementedException()
	};

}
