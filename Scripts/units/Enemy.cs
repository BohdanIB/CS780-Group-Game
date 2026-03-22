
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// TODO: Merge functionality between turrets and enemies somehow (shooter component which gets plugged into enemy and turret?)
///   - Shares a lot of functions with turret honestly...
/// </summary>
public partial class Enemy: PathFollower
{
	[Export] private EnemyStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.Weak;

	[Export] private ShooterComponent _shooterComponent;

	protected List<Area2D> _targetsInRange = new(); // todo - enemy needs a list of valid target types? (hit component probably deals with this)

	/// <summary>
	/// Initializes enemy with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(EnemyStats.Category type)
	{
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
		if (_stats != null)
		{
			Initialize(_stats);
		}

		_healthComponent.OnNoHealthLeft += () =>
		{
			GD.Print($"Enemy {Name} died.");
			QueueFree();
		};
		_hurtComponent.OnHurt += (area, damage) => { _healthComponent.ApplyDamage(damage); }; 

		_detectableComponent.OnDetected += (area) => {
			// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
			// {
			// 	GD.Print($"Enemy '{Name}' detected by '{owner.Name}'.");
			// }
			// else
			// {
			// 	GD.Print($"Enemy '{Name}' detected by '{area.Name}'.");
			// }
		};
		_detectableComponent.OnUnDetected += (area) => {
			// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
			// {
			// 	GD.Print($"Enemy '{Name}' UNdetected by '{owner.Name}'.");
			// }
			// else
			// {
			// 	GD.Print($"Enemy '{Name}' UNdetected by '{area.Name}'.");
			// }
		};

		_moverComponent.OnPathCompleted += () => {
			// GD.Print($"Enemy '{Name}' completed their path.");
		};

		_shooterComponent.OnShoot += () =>
		{
			GD.Print($"Shooter for enemy '{Name}' shooting");
		};

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		// Todo: Shoot at friendlies in range. Components?
		// FireAtValidTarget();
	}

	public void SetPath(Vector2[] path)
	{
		_moverComponent.SetPath(path);
	}

	public void UpdateStats(EnemyStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		UpdateHitboxRadius(_stats.HitboxRadius);
		UpdateDetectorRadius(_stats.AggroRadius);
		UpdateDetectableRadius(_stats.DetectableRadius);
		UpdateProjectileStats(_stats.ProjectileStats);
		UpdateSprite(); // todo: should be a component?

		// Todo: Add more updates

		UpdateHealth(_stats.Health);
	}
	protected void UpdateProjectileStats(ProjectileStats newStats)
	{
		_shooterComponent.Initialize(this, newStats);
	}
	private void UpdateSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	public override string ToString()
	{
		return $"Enemy '{Name}': {_stats}";
	}

/*
	/// <summary>
	/// TODO - This is a temporary function for testing enemy functionality on the game. This should go away at some point.
	/// 
	/// Spawn enemies at random tiles with dead-end roads to head towards hub
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="grid"></param>
	/// <param name="hub"></param>
	/// <param name="randomizer"></param>
	public static void TempEnemyDemo(Node parent, GenericGrid<GroundTile> grid, Vector2I hub, Random randomizer)
	{
		GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid, 
			(x,y) => {
				List<Vector2I> neighborPositions = [];
				if (grid.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); // UP
				if (grid.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); // RIGHT
				if (grid.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); // DOWN
				if (grid.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); // LEFT

				const float ROAD_COST = 0f;
				Dictionary<Vector2I, float> neighborCosts = [];

				GroundTile currentTile = grid.GetGridValueOrDefault(x, y);
				foreach (Vector2I coordinate in neighborPositions)
				{
					GroundTile nextTile = grid.GetGridValueOrDefault(coordinate.X, coordinate.Y);
					neighborCosts.Add(coordinate, currentTile.HasRoadConnection(nextTile.position - currentTile.position) ? ROAD_COST : int.MaxValue);
				}

				return neighborCosts;
			}
		);

		List<GroundTile> potentialEnemySpawnPoints = [];
		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				GroundTile t = grid.GetGridValueOrDefault(x, y);
				if (t.HasRoadDeadEnd())
				{
					potentialEnemySpawnPoints.Add(t);
				}
			}
		}

		List<Enemy> testEnemies = [];
		for (int i = 0; i < 6; i++)
		{
			var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
			enemy.Initialize(i == 0 ? EnemyStats.Category.Strong : EnemyStats.Category.Regular); // Make one 'strong' enemy for testing
			testEnemies.Add(enemy);
			parent.GetTree().GetRoot().CallDeferred("add_child", enemy); // Cannot add children in _Ready() calls

			// Set enemy paths
			var spawnPoint = potentialEnemySpawnPoints[randomizer.Next(potentialEnemySpawnPoints.Count)].position;
			var path = pathfinder.GetPathInPositions(spawnPoint, hub, grid.cellSize);
			enemy.SetPath(path);
			enemy.GlobalPosition = grid.GetCentralGridCellPositionPixels(spawnPoint);

			// GD.Print($"{enemy}");
		}
	}
*/

	// /// <summary>
	// /// If there is a valid target in range of current PathFollower, choose and fire at target given TargetingMode.
	// /// </summary>
	// private void FireAtValidTarget()
	// {
	// 	if (_targetsInRange.Count > 0 && _shotCooldownTimer.IsStopped())
	// 	{
	// 		Friendly currTarget;
	// 		if (_targetingMode == TargetingMode.Random)
	// 		{
	// 			currTarget = _targetsInRange[_random.Next(_targetsInRange.Count)];
	// 		}
	// 		else
	// 		{
	// 			// Look for an appropriate target to shoot at within PathFollower's radius given TargetingMode.
	// 			currTarget = _targetsInRange[0];
	// 			float currTargetDistanceFromSelf = Position.DistanceTo(currTarget.Position);
	// 			float currTargetDistanceFromGoal = currTarget.GetDistanceToGoalPixels();
	// 			float currTargetHealth = currTarget.GetCurrentHealth();
	// 			for (int i = 1; i < _targetsInRange.Count; i++)
	// 			{
	// 				var target = _targetsInRange[i];
	// 				float targetDistanceFromSelf = Position.DistanceTo(target.Position);
	// 				float targetDistanceFromGoal = target.GetDistanceToGoalPixels();
	// 				float targetHealth = target.GetCurrentHealth();
	// 				if ((_targetingMode == TargetingMode.First  && targetDistanceFromGoal < currTargetDistanceFromGoal) || 
	// 					(_targetingMode == TargetingMode.Last   && currTargetDistanceFromGoal < targetDistanceFromGoal) || 
	// 					(_targetingMode == TargetingMode.Close  && targetDistanceFromSelf < currTargetDistanceFromSelf) ||
	// 					(_targetingMode == TargetingMode.Weak   && targetHealth < currTargetHealth) ||
	// 					(_targetingMode == TargetingMode.Strong && currTargetHealth < targetHealth))
	// 				{
	// 					currTarget = target;
	// 					currTargetDistanceFromSelf = targetDistanceFromSelf;
	// 					currTargetDistanceFromGoal = targetDistanceFromGoal;
	// 					currTargetHealth = targetHealth;
	// 				}
	// 			}
	// 		}

	// 		// Shoot at the target
	// 		GD.Print($"Enemy {Name} firing Projectile at target {currTarget} with stats: {_stats.ProjectileStats}");
	// 		_shotCooldownTimer.Start(1 / _stats.FireRate);
	// 		var projectile = _projectileScene.Instantiate<Projectile>();
	// 		projectile.GlobalPosition = GlobalPosition;
	// 		projectile.Initialize(currTarget, _stats.ProjectileStats);
	// 		GetTree().GetRoot().AddChild(projectile);
	// 	}
	// }

}
