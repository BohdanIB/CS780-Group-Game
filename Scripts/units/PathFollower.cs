using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
public partial class PathFollower : Node2D
{
	public const Groups.GroupTypes TYPES = Groups.GroupTypes.None; // todo

	// Components //
	[ExportGroup("Components")]
	[Export] protected HealthComponent _health;
	[Export] protected HurtComponent _hurt;
	[Export] protected DetectorComponent _detector;
	[Export] protected DetectableComponent _detectable;
	[Export] protected MoverComponent _mover;
	[Export] protected AnimationComponent _animation;
	[Export] protected ShooterComponent _shooter;

	[Signal] public delegate void UnitDiedEventHandler(PathFollower unit);

	public override void _Ready()
	{
		if (_health == null || _hurt == null || _detector == null || _detectable == null || _mover == null || _animation == null)
		{
			GD.Print($"WARNING - PathFollower {this} was unable to find components on _Ready()");
		}
	}

	// // Change sprite to turn towards next path point
	// _idleAnimations.SetDirection(Position, _path[_currentPathIndex]);
	public void SetPath(Vector2[] path)
	{
		_mover.SetMoverPath(path);
	}

	public void StartMoving()
	{
		_mover.Start();
	}

	public void StopMoving()
	{
		_mover.Stop();
	}
}