
using Godot;
using System;
using System.Collections.Generic;

public partial class PathFollower : Area2D
{
	protected const float DISTANCE_THRESHOLD = 0.01f;

	// Scene Children
	[Export] protected Area2D _aggroArea2D;
	[Export] protected CollisionShape2D _aggroCollisionShape2D, _hitboxCollisionShape2D;
	[Export] protected AnimatedSprite2D _animatedSprite2D;
	[Export] protected Timer _shotCooldownTimer;

	// Preloaded Scenes
	[Export] protected PackedScene _projectileScene;

	protected float _health = 100.0f;
	protected float _movementSpeed = 50.0f;

	protected Random _random = new();
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
			}
			return;
		}

		Position = Position.MoveToward(_path[_currentPathIndex], (float) delta * _movementSpeed);
	}

	public void SetPath(List<Vector2> newPath)
	{
		_path = newPath;
		_currentPathIndex = 0;
	}

	public void ChangeHealth(float healthChangeValue)
	{
		GD.Print($"HealthChange for PathFollower {Name} - CurrentHealth {_health} -> NewHealth {_health - healthChangeValue}");
		_health -= healthChangeValue;
		if (_health <= 0.0f)
		{
			GD.Print($"PathFollower {Name} died.");
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
