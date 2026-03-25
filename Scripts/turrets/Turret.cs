
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;

/// <summary>
/// 
/// </summary>
// public partial class Turret : GenericStructure<TurretStats.Category>
public partial class Turret : GenericStructure
{
	[Export] private TurretStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.First;

	[ExportGroup("Exported Components")]
	[Export] private DetectorComponent _detectorComponent;
	[Export] private DetectableComponent _detectableComponent;
	// [Export] private SpawnerComponent _projectileSpawnerComponent;

	[ExportGroup("Exported Child Nodes")]
	[Export] private Timer _shotCooldownTimer;

	private bool _disabled = false; // todo: Get rid of this; this is garbage code
	private bool _visibleTurretRadius = true; // todo
	private bool _wasInitialized = false;

	// Preloaded Scenes
	// [Export] private PackedScene _projectileScene;

	private Random _random = new();
	private Godot.Collections.Array<Area2D> _targetsInRange = [];
	
	/// <summary>
	/// Initializes generic turret.
	/// </summary>
	/// <param name="turretType"></param>
	public void Initialize(TurretStats.Category turretType)
	{
		Initialize(new TurretStats(turretType));
	}
	/// <summary>
	/// Initialize generic turret with specific targeting mode.
	/// </summary>
	/// <param name="turretType"></param>
	/// <param name="targetingMode"></param>
	public void Initialize(TurretStats.Category turretType, TargetingMode targetingMode)
	{
		Initialize(turretType);
		_targetingMode = targetingMode;
	}
	/// <summary>
	/// Initialize turret with specific stats
	/// </summary>
	/// <param name="turretStats"></param>
	public void Initialize(TurretStats turretStats)
	{
		UpdateStats(turretStats);
	}

	public override void _Ready()
	{
		if (_stats != null)
		{
			Initialize(_stats);
		}

		// Todo: Primarily to support ghost mode turret in TurretPlacer. Probably better way of doing this.
		if (_disabled) { return; }

		// _hurtComponent.OnHurt += (area, damage) =>
		// {
		// 	_healthComponent.ApplyDamage(damage);
		// };
		_healthComponent.OnNoHealthLeft += () =>
		{
			GD.Print($"Turret {Name} died.");
			QueueFree();
		};

		// _detectorComponent.OnEnterDetector += (area) =>
		// {
		// 	// _targetsInRange = areasInDetectorRadius;
		// };
		// _detectorComponent.OnExitDetector += (area) =>
		// {
		// 	// _targetsInRange = areasInDetectorRadius;
		// };

		// GD.Print($"Turret Stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Todo: Primarily to support ghost mode turret in TurretPlacer.
		if (_disabled) { return; }

		// Shoot at an enemy if there is one in range.
		// ShootAtTargets();
	}

	// public void ShootAtTargets()
	// {
	// 	if (_enemiesInRange.Count > 0 && _shotCooldownTimer.IsStopped())
	// 	{
	// 		Enemy currTargetEnemy;
	// 		if (_targetingMode == TargetingMode.Random)
	// 		{
	// 			currTargetEnemy = _enemiesInRange[_random.Next(_enemiesInRange.Count)];
	// 		}
	// 		else
	// 		{
	// 			// Look for an appropriate enemy to shoot at within turret's radius given TargetingMode for turret.
	// 			currTargetEnemy = _enemiesInRange[0];
	// 			float currTargetDistanceFromTurret = Position.DistanceTo(currTargetEnemy.Position);
	// 			float currTargetDistanceFromGoal = currTargetEnemy.GetDistanceToGoalPixels();
	// 			float currTargetHealth = currTargetEnemy.GetCurrentHealth();
	// 			for (int i = 1; i < _enemiesInRange.Count; i++)
	// 			{
	// 				var enemy = _enemiesInRange[i];
	// 				float enemyDistanceFromTurret = Position.DistanceTo(enemy.Position);
	// 				float enemyDistanceFromGoal = enemy.GetDistanceToGoalPixels();
	// 				float enemyHealth = enemy.GetCurrentHealth();
	// 				if ((_targetingMode == TargetingMode.First  && enemyDistanceFromGoal < currTargetDistanceFromGoal) || 
	// 					(_targetingMode == TargetingMode.Last   && currTargetDistanceFromGoal < enemyDistanceFromGoal) || 
	// 					(_targetingMode == TargetingMode.Close  && enemyDistanceFromTurret < currTargetDistanceFromTurret) ||
	// 					(_targetingMode == TargetingMode.Weak   && enemyHealth < currTargetHealth) ||
	// 					(_targetingMode == TargetingMode.Strong && currTargetHealth < enemyHealth))
	// 				{
	// 					currTargetEnemy = enemy;
	// 					currTargetDistanceFromTurret = enemyDistanceFromTurret;
	// 					currTargetDistanceFromGoal = enemyDistanceFromGoal;
	// 					currTargetHealth = enemyHealth;
	// 				}
	// 			}
	// 		}
	// 		// Shoot at the target enemy
	// 		// GD.Print($"Turret {Name} firing Projectile at target {currTargetEnemy} with stats: {_stats.ProjectileStats}");
	// 		_shotCooldownTimer.Start(1 / _stats.FireRate);
	// 		var projectile = _projectileScene.Instantiate<Projectile>();
	// 		projectile.GlobalPosition = GlobalPosition;
	// 		projectile.Initialize(currTargetEnemy, _stats.ProjectileStats);
	// 		GetTree().GetRoot().AddChild(projectile);
	// 	}
	// }

	public override void _Draw()
	{
		if (_visibleTurretRadius)
		{
			DrawCircle(Vector2.Zero, _stats.AggroRadius, new Color(0xff000020), filled: true);
		}
	}

	public void UpdateTargetingMode(TargetingMode newMode)
	{
		_targetingMode = newMode;
	}

	/// <summary>
	/// Replace current stats with newStats, then update all necessary components which react to stat changes.
	/// </summary>
	/// <param name="newStats"></param>
	public void UpdateStats(TurretStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		SetHitboxRadius(_stats.HitboxRadius);
		SetDetectorRadius(_stats.AggroRadius);
		SetDetectableRadius(_stats.DetectableRadius);
		UpdateTurretSprite();

		// Todo: Add more updates

		UpdateTurretHealth(_stats.Health);

		// Redraw detector radius
		QueueRedraw();
	}
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
	private void UpdateTurretSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	private void UpdateTurretHealth(float newHealth)
	{
		_healthComponent.SetHealth(newHealth);
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
