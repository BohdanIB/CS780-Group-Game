
using System;
using System.Collections.Generic;
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class Enemy: PathFollower
{
	[Export] private EnemyStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.Weak;

	[Signal] public delegate void UnitReachedGoalEventHandler(int damage);

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
	[Export] private int _goalDamage = 10;

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
			_mover.Stop();
			if(_stats.Animations != null && _animation != null)
			{
				_animation.SetState(AnimationPackEntry.State.Death);
				_animation.SetDirection(_mover.LastDirection.Angle());
				string animName = _animation.Animation;
				_animation.SpriteFrames.SetAnimationLoop(animName, false);
				_animation.AnimationFinished += () => QueueFree();
			}
			else 
			{
				QueueFree();
			}
			EmitSignal(SignalName.UnitDied, this);
		};
		_hurt.OnHurt += (area, damage) => 
		{
			
			_health.ApplyDamage(damage); 
		}; 
		
		_mover.OnFreeze += () =>
		{
			_animation.Modulate = new Color(0.5f, 0.7f, 1.0f, 1.0f); // blueish tint
		};

		_mover.OnUnfreeze += () =>
		{
			_animation.Modulate = new Color(1f, 1f, 1f, 1f); // reset to normal
		};

		_mover.OnPathPointReached += (hasNextPoint, nextPoint) =>
		{
			if (hasNextPoint)
			{
				var directionRads = GlobalPosition.AngleToPoint(nextPoint);
				_animation.SetState(AnimationPackEntry.State.Bob);
				_animation.SetDirection(directionRads);
			}
		};
		_mover.OnPathCompleted += () =>
		{
			_animation.SetState(AnimationPackEntry.State.Idle);
		};

		_mover.OnPathCompleted += HandleReachedGoal;

	}

	public void UpdateStats(EnemyStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}

	private void InitializeComponents()
	{
		if (_stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_enemyTypes, _targetTypes);
			_detector.Initialize(_enemyTypes, _targetTypes);
			_detectable.Initialize(_enemyTypes, _targetTypes);
			_mover.Initialize(_stats.MovementSpeed, this, start: true);
			_shooter.Initialize(_stats.FireRate, _enemyTypes, _targetTypes, _stats.ProjectileStats);
			_animation.Initialize(_stats.Animations);
		}
	}
	private void UpdateComponents()
	{
		if (_stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_mover.Speed = _stats.MovementSpeed;
			_shooter.SetProjectileStats(_stats.ProjectileStats);
			_animation.Animations = _stats.Animations;
		}
	}

	public override string ToString()
	{
		return $"Enemy '{Name}': {_stats}";
	}

	/// <summary>
	/// TODO - This is a temporary function for testing enemy functionality on the game. This should go away at some point.
	/// 
	/// Spawn enemies at random tiles with dead-end roads to head towards hub
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="grid"></param>
	/// <param name="hub"></param>
	public static void TempEnemyDemo(Node parent, GenericGrid<GroundTile> grid, IsometricTileMap tileMap, Vector2I hub)
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

		var layers = tileMap.GetLayers();
		if (layers.Length <= 0)
		{
			GD.Print("WARNING: COULD NOT RUN ENEMY TEMP DEMO - NO LAYERS IN TILE MAP!");
			return;
		}
		var layer = layers[0];
		List<Enemy> testEnemies = [];
		for (int i = 0; i < 6; i++)
		{
			// Set enemy as child of parent
			var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
			parent.CallDeferred("add_child", enemy); // Cannot add children in _Ready() calls

			// Initialize
			testEnemies.Add(enemy);
			foreach(var stats in EnemyStats.ALL_ENEMIES)
			{
				if (stats.Type == EnemyStats.Category.Regular)
				{
					enemy.Initialize(stats);
					break;
				}
			}

			// Set enemy paths
			var spawnPoint = potentialEnemySpawnPoints[GD.RandRange(0, potentialEnemySpawnPoints.Count-1)].position;
			// var path = pathfinder.GetPathInPositions(spawnPoint, hub, grid.cellSize);
			var path = new List<Vector2>();
			foreach (var point in pathfinder.GetPath(spawnPoint, hub))
			{
				path.Add(IsometricTileMap.MapCoordToGlobalPosition(layer, point));
			}
			enemy.SetPath(path.ToArray());
			enemy.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(layer, spawnPoint);

			// GD.Print($"{enemy}");
		}
	}


	private void HandleReachedGoal()
	{
		EmitSignal(SignalName.UnitReachedGoal, _goalDamage);
		QueueFree();
	}

}
