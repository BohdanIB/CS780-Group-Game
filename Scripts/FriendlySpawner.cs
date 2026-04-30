using Godot;
using System;
using System.Collections.Generic;

public partial class FriendlySpawner : Node
{
	public static FriendlySpawner Instance {get; private set;}
	[Export] public float SpawnIntervalSeconds = 10f;
	[Export] public int FriendliesPerInterval = 3;

	private GenericGrid<GroundTile> _grid;
	private Vector2I _hub;
	private TileMapLayer _tileMapLayer;
	private GridAStarPathfinder<GroundTile> _pathfinder;
	private Timer _spawnTimer;

	private List<TradingPort> _ports;
	private int _portDispatchIndex;

	public void Initialize(GenericGrid<GroundTile> grid, Vector2I hub, TileMapLayer tileMapLayer)
	{
		_grid = grid;
		_hub = hub;
		_tileMapLayer = tileMapLayer;

		Instance ??= this;
		_ports = [];

		BuildPathfinder();

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

	private void SpawnGroup()
	{
		for (int i = 0; i < Math.Min(FriendliesPerInterval, _ports.Count); i++)
			SpawnFriendly(_ports[_portDispatchIndex]);

			_portDispatchIndex = (_portDispatchIndex + 1) % _ports.Count;
	}

	private void SpawnSingle()
	{
		SpawnFriendly(_ports[_portDispatchIndex]);

		_portDispatchIndex = (_portDispatchIndex + 1) % _ports.Count;
	}

	private void SpawnFriendly(TradingPort destinationPort)
	{
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

		List<Vector2I> outboundGridPath = _pathfinder.GetPath(_hub, destinationPort.WaterAccessPoint);
		List<Vector2I> inboundGridPath = _pathfinder.GetPath(destinationPort.WaterAccessPoint, _hub);

		List<Vector2> outboundWorldPath = [];
		List<Vector2> inboundWorldPath = [];
		foreach (var point in outboundGridPath)
			outboundWorldPath.Add(IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, point));
		foreach (var point in inboundGridPath)
			inboundWorldPath.Add(IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, point));

		friendly.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_tileMapLayer, _hub);

		friendly.SetTradeRoute(destinationPort, [.. outboundWorldPath], [.. inboundWorldPath]);

		friendly.OnRouteCompleted += SpawnSingle;

		GD.Print($"FriendlySpawner spawned a friendly heading to {destinationPort.WaterAccessPoint}.");
	}

	public void RegisterPort(TradingPort port)
	{
		_ports.Add(port);
	}
}
