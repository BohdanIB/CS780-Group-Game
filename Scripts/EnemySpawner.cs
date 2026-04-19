using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
	[Export] public float WaveIntervalSeconds = 30f;
	[Export] public int EnemiesPerWave = 5;
	[Export] public int FinalWaveNumber = 10;
	[Export] public int EnemiesAddedPerWave = 5;

	public int CurrentWave { get; private set; } = 0;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hub;
	private Random _random;
	private List<GroundTile> _spawnPoints;
	private GridAStarPathfinder<GroundTile> _pathfinder;
	private Timer _waveTimer;
	private GameUi _gameUi;
	private TileMapLayer _tileMapLayer;

	private List<EnemyStats> _enemyStats;

	public void Initialize(GenericGrid<GroundTile> grid, Vector2I hub, Random randomizer, TileMapLayer tileMapLayer)
	{
		_grid = grid;
		_hub = hub;
		_random = randomizer;
		_tileMapLayer = tileMapLayer;

		_enemyStats = EnemyStats.LoadAllStats();

		BuildSpawnPointList();
		BuildPathfinder();
		SpawnGoalSprite();

		_waveTimer = new Timer
		{
			WaitTime = WaveIntervalSeconds,
			OneShot = false
		};
		_waveTimer.Timeout += SpawnWave;
		AddChild(_waveTimer);
		_waveTimer.Start();

		_gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");

		GD.Print($"EnemySpawner initialized. First wave in {WaveIntervalSeconds} seconds.");
	}

	private void BuildSpawnPointList()
	{
		_spawnPoints = new List<GroundTile>();

		for (int x = 0; x < _grid.GetWidth(); x++)
		{
			for (int y = 0; y < _grid.GetHeight(); y++)
			{
				var tile = _grid.GetGridValueOrDefault(x, y);
				if (tile.HasRoadDeadEnd())
					_spawnPoints.Add(tile);
			}
		}

		GD.Print($"EnemySpawner found {_spawnPoints.Count} spawn points.");
	}

	private void BuildPathfinder()
	{
		_pathfinder = new GridAStarPathfinder<GroundTile>(
			_grid,
			(x, y) =>
			{
				List<Vector2I> neighbors = new();
				if (_grid.IsOnGrid(x, y - 1)) neighbors.Add(new Vector2I(x, y - 1));
				if (_grid.IsOnGrid(x + 1, y)) neighbors.Add(new Vector2I(x + 1, y));
				if (_grid.IsOnGrid(x, y + 1)) neighbors.Add(new Vector2I(x, y + 1));
				if (_grid.IsOnGrid(x - 1, y)) neighbors.Add(new Vector2I(x - 1, y));

				Dictionary<Vector2I, float> costs = new();
				var current = _grid.GetGridValueOrDefault(x, y);

				foreach (var pos in neighbors)
				{
					var next = _grid.GetGridValueOrDefault(pos.X, pos.Y);
					costs[pos] = current.HasRoadConnection(next.position - current.position)
						? 0f
						: float.MaxValue;
				}

				return costs;
			}
		);
	}

	private void SpawnGoalSprite()
	{
		var goalScene = GD.Load<PackedScene>("res://Scenes/base_scene.tscn");
		var goal = goalScene.Instantiate<Node2D>();

		Vector2 worldPos = IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, _hub);
		goal.Position = worldPos;

		var main = GetTree().Root.GetNode("Main");
		main.AddChild(goal);

		GD.Print($"Goal sprite placed at hub: {_hub}");
	}

	private void SpawnWave()
	{
		if (CurrentWave >= FinalWaveNumber)
		{
			GD.Print("Final wave reached. No more enemies will spawn.");
			GD.Print($"Player survived {CurrentWave} waves!");
			_waveTimer.Stop();
			return;
		}

		EnemiesPerWave += EnemiesAddedPerWave;

		GD.Print($"Spawning wave {CurrentWave + 1} with {EnemiesPerWave} enemies...");
		for (int i = 0; i < EnemiesPerWave; i++)
			SpawnSingleEnemy();

		CurrentWave++;
	}

	private void SpawnSingleEnemy()
	{
		if (_spawnPoints.Count == 0)
		{
			GD.PrintErr("EnemySpawner: No spawn points found!");
			return;
		}

		var spawnTile = _spawnPoints[_random.Next(_spawnPoints.Count)];
		var spawnPos = spawnTile.position;

		var enemyScene = GD.Load<PackedScene>("res://Scenes/enemy.tscn");
		var enemy = enemyScene.Instantiate<Enemy>();

		var regularStats = _enemyStats.Find(s => s.Type == EnemyStats.Category.Regular);
		if (regularStats == null)
		{
			GD.PrintErr("EnemySpawner: No EnemyStats with Type == Regular found!");
			return;
		}

		enemy.Initialize(regularStats);

		var pathGrid = _pathfinder.GetPath(spawnPos, _hub);
		if (pathGrid == null || pathGrid.Count == 0)
		{
			GD.PrintErr("EnemySpawner: No path found!");
			return;
		}

		List<Vector2> pathWorld = new();
		foreach (var p in pathGrid)
			pathWorld.Add(IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, p));

		enemy.GlobalPosition = pathWorld[0];
		enemy.SetPath(pathWorld.ToArray());
		enemy.StartMoving();

		var main = GetTree().Root.GetNode("Main");
		var enemiesParent = main.GetNode("Enemies");
		enemiesParent.CallDeferred("add_child", enemy);

		// ---------------------------------------------------------
		// CORRECT SIGNAL CONNECTIONS
		// ---------------------------------------------------------
		enemy.UnitDied += (_) => _gameUi.IncrementKillCount();
		enemy.UnitReachedGoal += (damage) => _gameUi.TakeDamage(damage);
	}

	private void OnEnemyReachedGoal()
	{
		_gameUi.TakeDamage(5);
	}

	// =========================================================
	// GAME RESET / CLEANUP SUPPORT
	// =========================================================

	public void CleanupDynamicNodes()
	{
		GD.Print("Cleaning up turrets, enemies, and projectiles...");

		foreach (Node turret in GetTree().GetNodesInGroup("placed_turrets"))
			turret.QueueFree();

		foreach (Node enemy in GetTree().GetNodesInGroup("enemies"))
			enemy.QueueFree();

		foreach (Node proj in GetTree().GetNodesInGroup("projectiles"))
			proj.QueueFree();
	}

	public void ResetGame()
	{
		CleanupDynamicNodes();

		CurrentWave = 0;
		EnemiesPerWave = 5;

		if (_waveTimer != null)
		{
			_waveTimer.Stop();
			_waveTimer.Start();
		}

		GD.Print("Game reset complete.");
	}
}
