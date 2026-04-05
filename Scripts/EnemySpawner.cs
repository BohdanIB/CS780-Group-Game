using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node
{
	[Export] public float WaveIntervalSeconds = 30f;
	[Export] public int EnemiesPerWave = 5;
	[Export] public int Final_wave_number = 10;

	[Export] public int Number_of_Enemies_added_per_wave = 5;

	public int CurrentWave { get; private set; } = 0;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hub;
	private Random _random;
	private List<GroundTile> _spawnPoints;
	private GridAStarPathfinder<GroundTile> _pathfinder;
	private Timer _waveTimer;
	private GameUi _gameUi;

	public void Initialize(GenericGrid<GroundTile> grid, Vector2I hub, Random randomizer)
	{
		_grid = grid;
		_hub = hub;
		_random = randomizer;

		BuildSpawnPointList();
		BuildPathfinder();

		_waveTimer = new Timer();
		_waveTimer.WaitTime = WaveIntervalSeconds;
		_waveTimer.OneShot = false;
		_waveTimer.Timeout += SpawnWave;
		AddChild(_waveTimer);
		_waveTimer.Start();
		_gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");

		GD.Print("EnemySpawner initialized. First wave in " + WaveIntervalSeconds + " seconds.");
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

	private void SpawnWave()
	{
		if(CurrentWave >= Final_wave_number)
		{
			GD.Print("Final wave reached. No more enemies will spawn.");
			GD.Print("Player survived " + CurrentWave + " waves!");
			_waveTimer.Stop();
			return;
		}
		EnemiesPerWave += Number_of_Enemies_added_per_wave;
		GD.Print("Spawning enemy wave...");
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

		var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
		enemy.Initialize(EnemyStats.Category.Regular);

		GetTree().GetRoot().CallDeferred("add_child", enemy);

		var path = _pathfinder.GetPathInPositions(spawnPos, _hub, _grid.cellSize);
		enemy.SetPath(path);

		enemy.GlobalPosition = _grid.GetCentralGridCellPositionPixels(spawnPos);
		enemy.UnitDied += (_) => {
			_gameUi.IncrementKillCount();};
			enemy.UnitReachedGoal += () => _gameUi.TakeDamage(5);
	}
}
