
using Godot;
using System;

public partial class AnimationManager : AnimatedSprite2D
{
	// private const string ANIMATIONS_DIRECTORY_PATH = "res://Assets/Animations/";

	private enum Direction
	{
		North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest,
	}
	// private static readonly Dictionary<Direction, string> DirectionAnimationNames = new()
	// {
	// 	{Direction.North,     "north"},
	// 	{Direction.NorthEast, "north_east"},
	// 	{Direction.East,      "east"},
	// 	{Direction.SouthEast, "south_east"},
	// 	{Direction.South,     "south"},
	// 	{Direction.SouthWest, "south_west"},
	// 	{Direction.West,      "west"},
	// 	{Direction.NorthWest, "north_west"},
	// };

	[Export] public SpriteFrames Frames {
		get => _frames; 
		set {
			_frames = value;
			this.SpriteFrames = _frames;
			this.Play(_frames.GetAnimationNames()[0]); // todo
		}
	}
	private SpriteFrames _frames;

	public override void _Ready()
	{
		this.SpriteFrames = Frames;
		
	}

	/// <summary>
	/// Given a point for this animation to look at from current
	/// </summary>
	/// <param name="directionRads"></param>
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

	// /// <summary>
	// /// Expecting rads from -PI to PI (where 0 corresponds to positive X, PI/2 corresponds to positive Y [WHICH IS DOWNWARD IN THE SCENE IN GODOT], etc.)
	// /// </summary>
	// /// <param name="rads"></param>
	// /// <returns></returns>
	// private static int RadsToFrameIndex(float rads) => Mathf.RadToDeg(rads) switch
	// {
	// 	< -157.5f => DirectionToFrameIndex(Direction.West),
	// 	< -112.5f => DirectionToFrameIndex(Direction.NorthWest),
	// 	< -67.5f  => DirectionToFrameIndex(Direction.North),
	// 	< -22.5f  => DirectionToFrameIndex(Direction.NorthEast),
	// 	< 22.5f   => DirectionToFrameIndex(Direction.East),
	// 	< 67.5f   => DirectionToFrameIndex(Direction.SouthEast),
	// 	< 112.5f  => DirectionToFrameIndex(Direction.South),
	// 	< 157.5f  => DirectionToFrameIndex(Direction.SouthWest),
	// 	<= 180f   => DirectionToFrameIndex(Direction.West),
	// 	_ => throw new NotImplementedException(),
	// };

	// private static int DirectionToFrameIndex(Direction dir) => dir switch
	// {
	// 	Direction.South     => 0,
	// 	Direction.SouthWest => 1,
	// 	Direction.West      => 2,
	// 	Direction.NorthWest => 3,
	// 	Direction.North     => 4,
	// 	Direction.NorthEast => 5,
	// 	Direction.East      => 6,
	// 	Direction.SouthEast => 7,
	// 	_ => throw new NotImplementedException()
	// };

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

	// private static List<TurretStats> LoadTurretStats()
	// {
	// 	DirAccess directory = DirAccess.Open(TURRET_DIRECTORY_PATH);
	// 	if (directory == null) return null;

	// 	List<TurretStats> loadedTurrets = [];

	// 	directory.ListDirBegin();

	// 	foreach (var turretFileName in directory.GetFiles())
	// 	{
	// 		loadedTurrets.Add(ResourceLoader.Load<TurretStats>($"{TURRET_DIRECTORY_PATH}/{turretFileName}"));
	// 	}

	// 	directory.ListDirEnd();

	// 	return [.. loadedTurrets];
	// }

}
