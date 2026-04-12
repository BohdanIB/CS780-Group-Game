using Godot;
using System;

public partial class AnimationManager : AnimatedSprite2D
{
	private enum Direction
	{
		North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest,
	}

	[Export] public SpriteFrames Frames {
		get => _frames; 
		set {
			_frames = value;
		}
	}
	private SpriteFrames _frames;

	public override void _Ready()
	{
		if (_frames != null && _frames.GetAnimationNames().Length > 0)
		{
			this.SpriteFrames = _frames;
			this.Play(_frames.GetAnimationNames()[0]);
		}
	}

	public void SetDirection(Vector2 from, Vector2 to)
	{
		var dir = RadsToDirection(from.AngleToPoint(to));
		var animationName = DirectionToAnimationName(dir);
		Play(animationName);
	}

	private static Direction RadsToDirection(float rads) => Mathf.RadToDeg(rads) switch
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
		Direction.North => "north",
		Direction.NorthEast => "north_east",
		Direction.East => "east",
		Direction.SouthEast => "south_east",
		Direction.South => "south",
		Direction.SouthWest => "south_west",
		Direction.West => "west",
		Direction.NorthWest => "north_west",
		_ => throw new NotImplementedException()
	};
}
