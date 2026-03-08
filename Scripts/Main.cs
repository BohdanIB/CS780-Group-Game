using System;
using System.Collections.Generic;
using Godot;

public partial class Main : Node2D
{
	[Export] public int MAIN_SEED = 12345;
	

	public override void _Ready()
	{
		Random randomizer = new(MAIN_SEED); // todo: might just want a global seeded randomizer singleton kicking around somewhere?

		var hubLocation = new Vector2I(20, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation, MAIN_SEED);

		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");
		gr.RenderGrid(grid);

		TurretPlacer turretPlacer = GetNode<TurretPlacer>("TurretPlacer");
		turretPlacer.Initialize(grid);

		// GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid, (tile) => tile == null ? -1 : 1, (x, y) =>
		// {
		// 	GroundTile tile = grid.GetGridValueOrDefault(x, y);
		// 	if (!tile.HasRoadConnection()) return [];
		// 	List<Vector2I> neighborCoordinates = [];
		// 	if (tile.HasRoadConnection(Vector2I.Up)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Up);
		// 	if (tile.HasRoadConnection(Vector2I.Right)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Right);
		// 	if (tile.HasRoadConnection(Vector2I.Down)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Down);
		// 	if (tile.HasRoadConnection(Vector2I.Left)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Left);

		// 	return [.. neighborCoordinates];
		// });



		////////////////////////////////////////////////
		// TODO: Temporary enemy testing with turrets //
		////////////////////////////////////////////////

		// GD.Print($"SpawnPoints: {potentialSpawnPoints}");
		GridAStarPathfinder<GroundTile> pathFollower_Pathfinder = new GridAStarPathfinder<GroundTile>(grid, 
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

		// Spawn enemies and set paths towards hub
		{
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
				GetTree().GetRoot().CallDeferred("add_child", enemy); // Cannot add children in _Ready()

				// Set enemy paths
				var spawnPoint = potentialEnemySpawnPoints[randomizer.Next(potentialEnemySpawnPoints.Count)].position;
				var path = pathFollower_Pathfinder.GetPathInPositions(spawnPoint, hubLocation, grid.cellSize);
				enemy.SetPath(path);
				enemy.GlobalPosition = grid.GetCentralGridCellPositionPixels(spawnPoint);

				// GD.Print($"{enemy}");
			}
		}

		// Spawn friendlies
		{
			GroundTile friendlySpawnPoint = grid.GetGridValueOrDefault(hubLocation.X, hubLocation.Y);
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
				GetTree().GetRoot().CallDeferred("add_child", friendly); // Cannot add children in _Ready()

				// Set path
				var endPoint = potentialFriendlyEndpoints[randomizer.Next(potentialFriendlyEndpoints.Count)].position;
				var path = pathFollower_Pathfinder.GetPathInPositions(hubLocation, endPoint, grid.cellSize);
				friendly.SetPath(path);
				friendly.GlobalPosition = grid.GetCentralGridCellPositionPixels(hubLocation);

				// GD.Print($"{friendly}");
			}
		}

	}

}
