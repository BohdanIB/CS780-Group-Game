
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

	// Scene Children //
	[ExportGroup("Exported Child Nodes")]
	[Export] protected AnimatedSprite2D _animatedSprite2D;

	public override void _Ready()
	{
		if (_health == null || _hurt == null || _detector == null || _detectable == null || _mover == null)
		{
			GD.Print($"WARNING - PathFollower {this} was unable to find components on _Ready()");
		}
	}

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
