using Godot;
using System;
using System.Collections.Generic;

public partial class FriendlySpawner : Node
{
	[Export] public float SpawnIntervalSeconds = 10f;
	[Export] public int FriendliesPerInterval = 3;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hub;
	private TileMapLayer _tileMapLayer;
	private GridAStarPathfinder<GroundTile> _pathfinder;
	private List<GroundTile> _endpoints;
	private Timer _spawnTimer;

	public void Initialize(GenericGrid<GroundTile> grid, Vector2I hub, TileMapLayer tileMapLayer)
	{
		_grid = grid;
		_hub = hub;
		_tileMapLayer = tileMapLayer;

		BuildPathfinder();
		BuildEndpointList();

		_spawnTimer = new Timer
		{
			WaitTime = SpawnIntervalSeconds,
			OneShot = false
		};
		_spawnTimer.Timeout += SpawnGroup;
		AddChild(_spawnTimer);
		_spawnTimer.Start();

		GD.Print($"FriendlySpawner initialized. Spawning every {SpawnIntervalSeconds} seconds.");
	}

	private void BuildPathfinder()
	{
		_pathfinder = new GridAStarPathfinder<GroundTile>(_grid,
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

	private void BuildEndpointList()
	{
		_endpoints = new List<GroundTile>();
		for (int x = 0; x < _grid.GetWidth(); x++)
		{
			for (int y = 0; y < _grid.GetHeight(); y++)
			{
				var tile = _grid.GetGridValueOrDefault(x, y);
				if (tile.HasRoadDeadEnd())
					_endpoints.Add(tile);
			}
		}
		GD.Print($"FriendlySpawner found {_endpoints.Count} endpoints.");
	}

	private void SpawnGroup()
	{
		for (int i = 0; i < FriendliesPerInterval; i++)
			SpawnFriendly();
	}

	private void SpawnFriendly()
	{
		if (_endpoints.Count == 0)
		{
			GD.PrintErr("FriendlySpawner: No endpoints found!");
			return;
		}

		var stats = FriendlyStats.ALL_FRIENDLIES?.Find(s => s.Type == FriendlyStats.Category.Regular);
		if (stats == null)
		{
			GD.PrintErr("FriendlySpawner: No Regular FriendlyStats found!");
			return;
		}

		var friendly = GD.Load<PackedScene>("res://Scenes/friendly.tscn").Instantiate<Friendly>();
		var main = GetTree().GetRoot().GetNode("Main");
		main.CallDeferred("add_child", friendly);

		friendly.Initialize(stats);

		var endpoint = _endpoints[GD.RandRange(0, _endpoints.Count - 1)].position;
		var pathGrid = _pathfinder.GetPath(_hub, endpoint);

		var pathWorld = new List<Vector2>();
		foreach (var point in pathGrid)
			pathWorld.Add(IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, point));

		friendly.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, _hub);
		friendly.SetPath(pathWorld.ToArray());

		GD.Print($"FriendlySpawner spawned a friendly heading to {endpoint}.");
	}
}
