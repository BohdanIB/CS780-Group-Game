
using Godot;
using System;
using System.Collections.Generic;

public partial class PathFollower : Area2D
{
	protected const float DISTANCE_THRESHOLD = 0.01f;

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

	public override void _PhysicsProcess(double delta)
	{
		FollowCurrentPath(delta);
	}

	protected void FollowCurrentPath(double delta)
	{
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

	public void SetPath(List<Vector2> newPath)
	{
		_path = newPath;
		_currentPathIndex = 0;
	}

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

	public float GetDistanceToGoalPixels()
	{
		if (_path == null)
		{
			return 0.0f; // todo?
		}
		else if (_currentPathIndex >= _path.Count)
		{
			return 0.0f;
		}

		float distance = Position.DistanceTo(_path[_currentPathIndex]);
		for (int i = _currentPathIndex+1; i < _path.Count; i++)
		{
			distance += _path[i-1].DistanceTo(_path[i]);
		}
		// GD.Print($"Follower {Name} distance to goal: {distance}");
		return distance;
	}

}
