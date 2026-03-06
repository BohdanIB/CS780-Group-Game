using System;
using System.Collections.Generic;
using Godot;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		var hubLocation = new Vector2I(10, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(21, 21), hubLocation);

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

		Random randomizer = new();

		// Spawn enemies and set paths towards hub
		{
			GridAStarPathfinder<GroundTile> enemyPathfinder = new GridAStarPathfinder<GroundTile>(grid, 
				(tile) =>
				{
					return tile.HasRoadConnection() ? 9 : (-(Math.Abs(tile.position.X - hubLocation.X) + Math.Abs(tile.position.Y - hubLocation.Y)) + grid.GetWidth() + grid.GetHeight());
				},
				(x,y) => {
					List<Vector2I> neighborPositions = [];
					if (grid.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); 
					if (grid.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); 
					if (grid.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); 
					if (grid.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); 
					return [.. neighborPositions];
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
				GetTree().GetRoot().CallDeferred("add_child", enemy); // Cannot add children in _Ready()

				// Set enemy paths
				var spawnPoint = potentialEnemySpawnPoints[randomizer.Next(potentialEnemySpawnPoints.Count)].position;
				var path = enemyPathfinder.GetPathInPositions(spawnPoint, hubLocation, grid.cellSize);
				enemy.SetPath(path);
				enemy.GlobalPosition = grid.GetCentralGridCellPositionPixels(spawnPoint);

				// GD.Print($"{enemy}");
			}
		}

		// Spawn friendlies
		{
			GridAStarPathfinder<GroundTile> friendlyPathfinder = new GridAStarPathfinder<GroundTile>(grid, 
				(tile) =>
				{
					return tile.HasRoadConnection() ? 9 : (-(Math.Abs(tile.position.X - hubLocation.X) + Math.Abs(tile.position.Y - hubLocation.Y)) + grid.GetWidth() + grid.GetHeight());
				},
				(x,y) => {
					List<Vector2I> neighborPositions = [];
					if (grid.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); 
					if (grid.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); 
					if (grid.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); 
					if (grid.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); 
					return [.. neighborPositions];
				}
			);
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
				var path = friendlyPathfinder.GetPathInPositions(hubLocation, endPoint, grid.cellSize);
				friendly.SetPath(path);
				friendly.GlobalPosition = grid.GetCentralGridCellPositionPixels(hubLocation);

				GD.Print($"{friendly}");
			}
		}

	}

}
