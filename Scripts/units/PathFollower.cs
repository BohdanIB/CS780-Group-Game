using CS780GroupProject.Scripts.Utils;
using Godot;
using System;

public partial class PathFollower : Node2D
{
	public const Groups.GroupTypes TYPES = Groups.GroupTypes.None;

	[ExportGroup("Components")]
	[Export] protected HealthComponent _health;
	[Export] protected HurtComponent _hurt;
	[Export] protected DetectorComponent _detector;
	[Export] protected DetectableComponent _detectable;
	[Export] protected MoverComponent _mover;
	[Export] protected AnimationComponent _animation;

	[Export] protected Area2D _aggroArea2D;
	[Export] protected CollisionShape2D _aggroCollisionShape2D, _hitboxCollisionShape2D;
	[Export] protected Timer _shotCooldownTimer;

	[Export] protected PackedScene _projectileScene;

	[Signal] public delegate void UnitDiedEventHandler(PathFollower unit);
	[Signal] public delegate void UnitReachedGoalEventHandler(int damage);

	protected float _healthValue = 100.0f;

	protected Vector2[] _path = Array.Empty<Vector2>();
	protected int _currentPathIndex = 0;

	public override void _Ready()
	{
		if (_health == null || _hurt == null || _detector == null || _detectable == null || _mover == null || _animation == null)
		{
			GD.Print($"WARNING - PathFollower {Name} missing components.");
		}

		// Connect movement completion
		_mover.Connect(MoverComponent.SignalName.OnPathCompleted, new Callable(this, nameof(HandleReachedGoal)));
	}

	private void HandleReachedGoal()
	{
		EmitSignal(SignalName.UnitReachedGoal, 10);
		QueueFree();
	}

	public void SetPath(Vector2[] path)
	{
		_path = path;
		_currentPathIndex = 0;
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

	public void ChangeHealth(float amount)
	{
		_healthValue -= amount;
		if (_healthValue <= 0)
		{
			EmitSignal(SignalName.UnitDied, this);
			QueueFree();
		}
	}

	public float GetCurrentHealth() => _healthValue;
}
