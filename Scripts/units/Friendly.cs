
using Godot;
using System;
using System.Collections.Generic;

public partial class Friendly : PathFollower
{
	private FriendlyStats _stats;

	/// <summary>
	/// Initializes friendly with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(FriendlyStats.Category type)
	{
		Initialize(new FriendlyStats(type));
	}
	/// <summary>
	/// Initializes friendly with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(FriendlyStats stats)
	{
		UpdateStats(stats);
	}

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		// Todo
	}

	public void UpdateStats(FriendlyStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		_hurt.SetRadius(_stats.HitboxRadius);
		_detector.SetRadius(_stats.AggroRadius);
		// _detectable.SetRadius(1) // todo
		UpdateSprite(); // todo

		// Todo: Add more updates

		_health.SetHealth(_stats.Health);
	}
	private void UpdateSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	public override string ToString()
	{
		return $"Friendly '{Name}': {_stats}";
	}

	/*
	/// <summary>
	/// TODO - This is a temporary function for testing friendly functionality on the game. This should go away at some point.
	/// 
	/// Spawn friendly units at hub to head towards random tiles with dead-end road segments
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="grid"></param>
	/// <param name="hub"></param>
	/// <param name="randomizer"></param>
	public static void TempFriendlyDemo(Node parent, GenericGrid<GroundTile> grid, Vector2I hub, Random randomizer)
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

		GroundTile friendlySpawnPoint = grid.GetGridValueOrDefault(hub.X, hub.Y);
		List<GroundTile> potentialFriendlyEndpoints = [];
		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				GroundTile t = grid.GetGridValueOrDefault(x, y);
				if (t.HasRoadDeadEnd())
				{
					potentialFriendlyEndpoints.Add(t);
				}
			}
		}

		List<Friendly> testFriendlies = [];
		for (int i = 0; i < 3; i++)
		{
			var friendly = GD.Load<PackedScene>("res://Scenes/friendly.tscn").Instantiate<Friendly>();
			friendly.Initialize(i == 0 ? FriendlyStats.Category.Loaded : FriendlyStats.Category.Regular); // Make one 'loaded' enemy for testing
			testFriendlies.Add(friendly);
			parent.GetTree().GetRoot().CallDeferred("add_child", friendly); // Cannot add children in _Ready()

			// Set path
			var endPoint = potentialFriendlyEndpoints[randomizer.Next(potentialFriendlyEndpoints.Count)].position;
			var path = pathfinder.GetPathInPositions(hub, endPoint, grid.cellSize);
			friendly.SetPath(path);
			friendly.GlobalPosition = grid.GetCentralGridCellPositionPixels(hub);

			// GD.Print($"{friendly}");
		}
		
	}
	*/

}
