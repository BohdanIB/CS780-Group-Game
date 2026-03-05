using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public partial class Turret : Area2D
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

	// [Export] private TurretStats.Category _turretType = TurretStats.Category.Balista;
	[Export] private bool _disabled = false;
	[Export] private bool _visibleTurretRadius = true;
	[Export] private TargetingMode _targetingMode = TargetingMode.First;
	[Export] private TurretStats _stats;

	// Scene Children
	// private CollisionShape2D _collisionShape2D;
	// private AnimatedSprite2D _sprite;
	private Timer _shotCooldownTimer;

	private Random _random = new();
	private List<PathFollower> _enemiesInRange = new();

	/// <summary>
	/// Initializes turret with custom stats.
	/// </summary>
	/// <param name="turretStats"></param>
	public void Initialize(TurretStats turretStats)
	{
		_stats = turretStats;
	}
	/// <summary>
	/// Initializes turret with "generic" base stats for given type.
	/// </summary>
	/// <param name="turretType"></param>
	public void Initialize(TurretStats.Category turretType)
	{
		Initialize(new TurretStats(turretType));
	}
	public void Initialize(TurretStats.Category turretType, TargetingMode targetingMode)
	{
		_targetingMode = targetingMode;
		Initialize(turretType);
	}

	public override void _Ready()
	{
		// Some cases where _Ready gets called before Initialize, just set to some value for now and it will get reinitialized later.
		if (_stats == null)
		{
			Initialize(TurretStats.Category.Ballista);
		}
		UpdateStats(_stats);

		// _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		// UpdateTurretRadius(_stats.AggroRadius);

		// _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		// UpdateTurretSprite(_stats.SpriteFrame);

		_shotCooldownTimer = GetNode<Timer>("ShotCooldownTimer");

		// Todo: Primarily to support ghost mode turret in TurretPlacer. Probably better way of doing this.
		if (_disabled) { return; }

		AreaEntered += (area) => {
			if (area is PathFollower pf)
			{
				GD.Print($"TURRET BODY ENTERED: {pf.Name}");
				_enemiesInRange.Add(pf);
			}
		};
		AreaExited += (area) => // todo: Case where path follower dies within area? Does it still send exit signal?
		{
			if (area is PathFollower pf)
			{
				GD.Print($"TURRET BODY EXITED: {pf.Name}");
				_enemiesInRange.Remove(pf);
			}
		};

		// GD.Print($"Turret Stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Todo: Primarily to support ghost mode turret in TurretPlacer.
		if (_disabled) { return; }

		// Check if something is in area of turret
		// If something is in area, start shooting at target (spawn Projectile?)
		//   Projectile should send signal to shot path follower when it gets hit (damage)
		// Obey firerate restrictions

		// Shoot at an enemy if there is one in range.
		// TODO: Should target enemies based on how far they are along path.
		if (_enemiesInRange.Count > 0 && _shotCooldownTimer.IsStopped())
		{
			PathFollower currTargetEnemy;
			if (_targetingMode == TargetingMode.Random)
			{
				currTargetEnemy = _enemiesInRange[_random.Next(_enemiesInRange.Count)];
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
			var projectile = GD.Load<PackedScene>("res://Scenes/projectile.tscn").Instantiate<Projectile>();
			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(currTargetEnemy, _stats.ProjectileStats);
			GetTree().GetRoot().AddChild(projectile);
		}
	}

	public override void _Draw()
	{
		if (_visibleTurretRadius)
		{
			DrawCircle(Vector2.Zero, _stats.AggroRadius, new Color(0xff000020), filled: true);
		}
	}

	/// <summary>
	/// Replace current stats with newStats, then update all necessary components which react to stat changes.
	/// </summary>
	/// <param name="newStats"></param>
	public void UpdateStats(TurretStats newStats)
	{
		_stats = newStats;
		UpdateTurretRadius(_stats.AggroRadius);
		UpdateTurretSprite(_stats.SpriteFrame);

		// Redraw aggro radius
		QueueRedraw();
	}
	private void UpdateTurretRadius(float newRadius)
	{
		_stats.AggroRadius = newRadius;
		// ((CircleShape2D)_collisionShape2D.Shape).Radius = newRadius; // TODO: Better way of doing this?
		((CircleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape).Radius = newRadius; // TODO: Better way of doing this?
	}
	private void UpdateTurretSprite(int newFrame)
	{
		_stats.SpriteFrame = newFrame;
		// _sprite.Frame = newFrame;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Frame = newFrame;
	}

	public void UpdateTargetingMode(TargetingMode newMode)
	{
		_targetingMode = newMode;
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
