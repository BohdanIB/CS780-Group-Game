
using System;
using System.Collections.Generic;
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class Enemy: PathFollower
{
	[Export] private TargetingMode _targetingMode = TargetingMode.Weak;
	[Export] private EnemyStats _stats;

	[ExportGroup("Types")]
	[Export] public Groups.GroupTypes _enemyTypes = PathFollower.TYPES | Groups.GroupTypes.Enemy;
	[Export] public Groups.GroupTypes _targetTypes = Groups.GroupTypes.Friendly | Groups.GroupTypes.Turret | Groups.GroupTypes.Structure;
	// [Export] public Groups.GroupTypes _hurtTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectorTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectableTypes = Groups.GroupTypes.Enemy;

	[ExportGroup("Components")]
	[Export] private ShooterComponent _shooter;
	[Export] private TargetingComponent _targeting;
	[Export] protected SpawnerComponent _projectileSpawner;

	/// <summary>
	/// Initializes enemy with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(EnemyStats.Category type, Vector2[] path = null)
	{
		SetPath(path);
		Initialize(new EnemyStats(type));
	}
	/// <summary>
	/// Initializes enemy with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(EnemyStats stats)
	{
		UpdateStats(stats);
	}

	public override void _Ready()
	{
		base._Ready();

		if (_shooter == null || _targeting == null || _projectileSpawner == null)
		{
			GD.Print($"WARNING - Enemy {this} was unable to find one of its components on _Ready()");
		}

		if (_stats != null)
		{
			Initialize(_stats);
		}

		// Component callbacks //

		_health.OnNoHealthLeft += () =>
		{
			GD.Print($"Enemy {Name} died.");
			QueueFree();
		};
		_hurt.OnEnterHurt += (area, damage) => 
		{
			_health.ApplyDamage(damage); 
		}; 

		// _detectable.OnEnterDetectable += (detector) => {
		// 	// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
		// 	// {
		// 	// 	GD.Print($"Enemy '{Name}' detected by '{owner.Name}'.");
		// 	// }
		// 	// else
		// 	// {
		// 	// 	GD.Print($"Enemy '{Name}' detected by '{area.Name}'.");
		// 	// }
		// };
		// _detectable.OnExitDetectable += (detector) => {
		// 	// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
		// 	// {
		// 	// 	GD.Print($"Enemy '{Name}' UNdetected by '{owner.Name}'.");
		// 	// }
		// 	// else
		// 	// {
		// 	// 	GD.Print($"Enemy '{Name}' UNdetected by '{area.Name}'.");
		// 	// }
		// };

		// _mover.OnPathCompleted += () => {
		// 	// GD.Print($"Enemy '{Name}' completed their path.");
		// };

		// _shooterComponent.OnShoot += () =>
		// {
		// 	GD.Print($"Shooter for enemy '{Name}' shooting");
		// };

		CallDeferred(MethodName.InitializeComponents);

	}

	// public override void _PhysicsProcess(double delta)
	// {
	// 	base._PhysicsProcess(delta);
	// 	// Todo: Shoot at friendlies in range. Components?
	// 	// FireAtValidTarget();
	// }

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

	public void UpdateStats(EnemyStats newStats)
	{
		_stats = newStats;
		UpdateComponents();
	}
	public void UpdateComponents()
	{
		if (this.IsNodeReady())
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_mover.Speed = _stats.MovementSpeed;
			_shooter.SetProjectileStats(_stats.ProjectileStats);
			
			// Todo: Add more updates
		}
		UpdateSprite(); // todo: should be a component?
	}

	private void InitializeComponents()
	{
		if (this.IsNodeReady())
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_enemyTypes, _targetTypes);
			_detector.Initialize(_enemyTypes, _targetTypes);
			_detectable.Initialize(_enemyTypes, _targetTypes);
			_mover.Initialize(_stats.MovementSpeed, this, start: true);
			_shooter.Initialize(_stats.FireRate, _enemyTypes, _stats.ProjectileStats);
		}
	}
	// protected void UpdateProjectileStats(ProjectileStats newStats)
	// {
	// 	_shooterComponent.SetProjectileStats(newStats);
	// }
	private void UpdateSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	public override string ToString()
	{
		return $"Enemy '{Name}': {_stats}";
	}

}
