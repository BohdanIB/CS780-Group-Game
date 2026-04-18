
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
	// Scene Children
	[Export] protected Area2D _aggroArea2D;
	[Export] protected CollisionShape2D _aggroCollisionShape2D, _hitboxCollisionShape2D;
	[Export] protected Timer _shotCooldownTimer;
	[Export] protected AnimationManager _idleAnimations;

	// Preloaded Scenes
	[Export] protected PackedScene _projectileScene;
	[Signal]public delegate void UnitDiedEventHandler(PathFollower unit);
	[Signal] public delegate void UnitReachedGoalEventHandler();

	protected float _health = 100.0f;
	[Export]
	protected float _movementSpeed = 150.0f;

	protected List<Vector2> _path;
	protected int _currentPathIndex;

	public override void _Ready()
	{
		if (_health == null || _hurt == null || _detector == null || _detectable == null || _mover == null || _animation == null)
		{
			GD.Print($"WARNING - PathFollower {this} was unable to find components on _Ready()");
		}
	}

		// // Change sprite to turn towards next path point
		// _idleAnimations.SetDirection(Position, _path[_currentPathIndex]);
		if (_path == null) return;
		if (Position.DistanceTo(_path[_currentPathIndex]) < DISTANCE_THRESHOLD)
		{
			_currentPathIndex++;
			if (_currentPathIndex >= _path.Count)
			{
				_path = null;
				GD.Print($"PathFollower {Name} reached end of path.");
				EmitSignal(SignalName.UnitReachedGoal);
				QueueFree();
			}
			return;
		}

		Position = Position.MoveToward(_path[_currentPathIndex], (float) delta * _movementSpeed); // This results in jittery movement overshoots path points, but this is fixed in ECS PR.

		// Change sprite to turn towards next path point
		_idleAnimations.SetDirection(Position, _path[_currentPathIndex]);
	}

	public void SetPath(Vector2[] path)
	{
		_mover.SetMoverPath(path);
	}
	public void StartMoving()
	{
		_mover.Start();

	public void ChangeHealth(float healthChangeValue)
	{
		// GD.Print($"HealthChange for PathFollower {Name} - CurrentHealth {_health} -> NewHealth {_health - healthChangeValue}");
		_health -= healthChangeValue;
		if (_health <= 0.0f)
		{
			//GD.Print($"PathFollower {Name} died.");
			EmitSignal(SignalName.UnitDied, this);
			QueueFree();
		}
	}

	public float GetCurrentHealth()
	{
		return _health;
	}
	public void StopMoving()
	{
		_mover.Stop();
	}

}
