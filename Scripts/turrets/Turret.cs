
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
// public partial class Turret : GenericStructure<TurretStats.Category>
public partial class Turret : GenericStructure
{
	// Stolen from BloonsTD; Targeting priorities for turret
	public enum TargetingMode
	{
		Random, // Random
		First,  // Furthest down path
		Last,   // Closest to spawn
		Close,  // Closest to tower
		Weak,   // Weakest enemies
		Strong, // Strongest enemies
	}

	private bool _disabled = false;
	private bool _visibleTurretRadius = true;
	[Export] private TurretStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.First;

	// Scene Children
	[Export] private Timer _shotCooldownTimer;

	// Preloaded Scenes
	[Export] private PackedScene _projectileScene;

	private List<Enemy> _enemiesInRange = new();

	/// <summary>
	/// Initializes turret with given stats.
	/// </summary>
	/// <param name="turretStats"></param>
	public void Initialize(TurretStats turretStats, TargetingMode targetingMode = TargetingMode.First)
	{
		_targetingMode = targetingMode;
		UpdateStats(turretStats);
	}

	public override void _Ready()
	{
		UpdateStats(_stats);

		// Todo: Primarily to support ghost mode turret in TurretPlacer. Probably better way of doing this.
		if (_disabled) { return; }

		AreaEntered += (area) => {
			if (area is Enemy pf)
			{
				// GD.Print($"TURRET BODY ENTERED BY PATH FOLLOWER '{pf.Name}'");
				_enemiesInRange.Add(pf);
			}
		};
		AreaExited += (area) => // todo: Case where path follower dies within area? Does it still send exit signal?
		{
			if (area is Enemy pf)
			{
				// GD.Print($"TURRET BODY EXITED BY PATH FOLLOWER '{pf.Name}'");
				_enemiesInRange.Remove(pf);
			}
		};

		// GD.Print($"Turret Stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Todo: Primarily to support ghost mode turret in TurretPlacer.
		if (_disabled) { return; }

		// Shoot at an enemy if there is one in range.
		if (_enemiesInRange.Count > 0 && _shotCooldownTimer.IsStopped())
		{
			Enemy currTargetEnemy;
			if (_targetingMode == TargetingMode.Random)
			{
				currTargetEnemy = _enemiesInRange[GD.RandRange(0, _enemiesInRange.Count-1)];
			}
			else
			{
				// Look for an appropriate enemy to shoot at within turret's radius given TargetingMode for turret.
				currTargetEnemy = _enemiesInRange[0];
				float currTargetDistanceFromTurret = Position.DistanceTo(currTargetEnemy.Position);
				float currTargetDistanceFromGoal = currTargetEnemy.GetDistanceToGoalPixels();
				float currTargetHealth = currTargetEnemy.GetCurrentHealth();
				for (int i = 1; i < _enemiesInRange.Count; i++)
				{
					var enemy = _enemiesInRange[i];
					float enemyDistanceFromTurret = Position.DistanceTo(enemy.Position);
					float enemyDistanceFromGoal = enemy.GetDistanceToGoalPixels();
					float enemyHealth = enemy.GetCurrentHealth();
					if ((_targetingMode == TargetingMode.First  && enemyDistanceFromGoal < currTargetDistanceFromGoal) || 
						(_targetingMode == TargetingMode.Last   && currTargetDistanceFromGoal < enemyDistanceFromGoal) || 
						(_targetingMode == TargetingMode.Close  && enemyDistanceFromTurret < currTargetDistanceFromTurret) ||
						(_targetingMode == TargetingMode.Weak   && enemyHealth < currTargetHealth) ||
						(_targetingMode == TargetingMode.Strong && currTargetHealth < enemyHealth))
					{
						currTargetEnemy = enemy;
						currTargetDistanceFromTurret = enemyDistanceFromTurret;
						currTargetDistanceFromGoal = enemyDistanceFromGoal;
						currTargetHealth = enemyHealth;
					}
				}
			}
			// Shoot at the target enemy
			// GD.Print($"Turret {Name} firing Projectile at target {currTargetEnemy} with stats: {_stats.ProjectileStats}");
			_shotCooldownTimer.Start(1 / _stats.FireRate);
			var projectile = _projectileScene.Instantiate<Projectile>();
			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(currTargetEnemy, _stats.ProjectileStats);
			GetTree().GetRoot().AddChild(projectile);

			// Change sprite to turn towards target
			_idleAnimations.SetDirection(GlobalPosition, currTargetEnemy.GlobalPosition);
		}
	}

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
	public void UpdateStats(TurretStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateTurretRadius();
		UpdateTurretSpriteFrames();

		// Todo: Add more updates

		UpdateTurretHealth();

		// Redraw aggro radius
		QueueRedraw();
	}
	private void UpdateTurretRadius()
	{
		((CircleShape2D)_collisionShape2D.Shape).Radius = _stats.AggroRadius; // TODO: Better way of doing this?
	}
	private void UpdateTurretSpriteFrames()
	{
		_idleAnimations.Frames = _stats.Animations.Idle; // TODO: This needs to be expanded in future
	}
	private void UpdateTurretHealth()
	{
		_health = _stats.Health;
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

    public override void SetConfigurationOption(string configurationName, string configurationSelection)
    {
		
        if (configurationName.Equals("Target"))
		{
			foreach (TargetingMode mode in Enum.GetValues(typeof(TargetingMode)))
			{
				if (mode.ToString().Equals(configurationSelection)) 
				{
					UpdateTargetingMode(mode);				
				}
			}
		}
    }


}
