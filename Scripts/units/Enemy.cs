
using System;
using System.Collections.Generic;
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class Enemy: PathFollower
{
	[Export] private TargetingMode _targetingMode = TargetingMode.Weak;
	[Export] private EnemyStats _stats = new(EnemyStats.Category.Regular);

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
	/// <param name="path"></param>
	public void Initialize(EnemyStats.Category type, Vector2[] path = null)
	{
		Initialize(new EnemyStats(type), path);
	}
	
	/// <summary>
	/// Initializes enemy with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	/// <param name="path"></param>
	public void Initialize(EnemyStats stats, Vector2[] path = null)
	{
		SetPath(path);
		_stats = stats;
		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		base._Ready();

		if (_shooter == null || _targeting == null || _projectileSpawner == null)
		{
			GD.Print($"WARNING - Enemy {this} was unable to find one of its components on _Ready()");
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

	}

	public void UpdateStats(EnemyStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}
	public void UpdateComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_mover.Speed = _stats.MovementSpeed;
			_shooter.SetProjectileStats(_stats.ProjectileStats);
			
			// Todo: Add more updates
			UpdateSprite(); // todo: should be a component?
		}
	}

	private void InitializeComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_enemyTypes, _targetTypes);
			_detector.Initialize(_enemyTypes, _targetTypes);
			_detectable.Initialize(_enemyTypes, _targetTypes);
			_mover.Initialize(_stats.MovementSpeed, this, start: true);
			_shooter.Initialize(_stats.FireRate, _enemyTypes, _targetTypes, _stats.ProjectileStats);

			UpdateSprite(); // todo: should be a component?
		}
	}

	private void UpdateSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	// protected void UpdateProjectileStats(ProjectileStats newStats)
	// {
	// 	_shooterComponent.SetProjectileStats(newStats);
	// }
	public override string ToString()
	{
		return $"Enemy '{Name}': {_stats}";
	}

}
