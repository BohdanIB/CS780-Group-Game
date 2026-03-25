
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class PathFollower : Node2D
{
	// protected const float DISTANCE_THRESHOLD = 0.01f;

	// Components //
	[ExportGroup("Exported Components")]
	[Export] protected HealthComponent _healthComponent;
	[Export] protected HurtComponent _hurtComponent;
	[Export] protected DetectorComponent _detectorComponent;
	[Export] protected DetectableComponent _detectableComponent;
	[Export] protected MoverComponent _moverComponent;

	// Scene Children //
	[ExportGroup("Exported Child Nodes")]
	[Export] protected AnimatedSprite2D _animatedSprite2D;

	// Preloaded Scenes //
	// [Export] protected PackedScene _projectileScene;

	// protected float _health = 100.0f;
	// protected float _movementSpeed = 50.0f;

	protected Random _random = new();
	// protected List<Area2D> _detectedEntities = new();

	public override void _Ready()
	{
		Debug.Assert(IsInstanceValid(_healthComponent));
		Debug.Assert(IsInstanceValid(_hurtComponent));
		Debug.Assert(IsInstanceValid(_detectorComponent));
		Debug.Assert(IsInstanceValid(_detectableComponent));
		Debug.Assert(IsInstanceValid(_moverComponent));
	}

	public override void _PhysicsProcess(double delta)
	{
		// FollowCurrentPath(delta);
	}

	// Todo
	// protected void FollowCurrentPath(float movementSpeed, double delta)
	// {
	// 	if (_path == null) return;
	// 	if (Position.DistanceTo(_path[_currentPathIndex]) < DISTANCE_THRESHOLD)
	// 	{
	// 		_currentPathIndex++;
	// 		if (_currentPathIndex >= _path.Count)
	// 		{
	// 			_path = null;
	// 			GD.Print($"PathFollower {Name} reached end of path.");
	// 		}
	// 		return;
	// 	}
	// 	Position = Position.MoveToward(_path[_currentPathIndex], (float) delta * movementSpeed);
	// }

	// public void SetPath(List<Vector2> newPath)
	// {
	// 	_path = newPath;
	// 	_currentPathIndex = 0;
	// }

	// public void ChangeHealth(float healthChangeValue)
	// {
		// GD.Print($"HealthChange for PathFollower {Name} - CurrentHealth {_health} -> NewHealth {_health - healthChangeValue}");
		// _health -= healthChangeValue;
		// if (_health <= 0.0f)
		// {
		// 	GD.Print($"PathFollower {Name} died.");
		// 	QueueFree();
		// }
	// }

	// public float GetCurrentHealth()
	// {
	// 	return _health;
	// }

	// public float GetDistanceToGoalPixels()
	// {
	// 	if (_path == null)
	// 	{
	// 		return 0.0f; // todo?
	// 	}
	// 	else if (_currentPathIndex >= _path.Count)
	// 	{
	// 		return 0.0f;
	// 	}

	// 	float distance = Position.DistanceTo(_path[_currentPathIndex]);
	// 	for (int i = _currentPathIndex+1; i < _path.Count; i++)
	// 	{
	// 		distance += _path[i-1].DistanceTo(_path[i]);
	// 	}
	// 	// GD.Print($"Follower {Name} distance to goal: {distance}");
	// 	return distance;
	// }


	protected void SetHitboxRadius(float newRadius)
	{
		_hurtComponent.SetRadius(newRadius);
	}
	protected void SetDetectorRadius(float newRadius)
	{
		_detectorComponent.SetRadius(newRadius);
	}
	protected void SetDetectableRadius(float newRadius)
	{
		_detectableComponent.SetRadius(newRadius);
	}
	// Todo
	protected void UpdateHealth(float newHealth)
	{
		_healthComponent.SetHealth(newHealth);
	}
}
