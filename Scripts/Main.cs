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

		List<GroundTile> potentialSpawnPoints = [];
		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				GroundTile t = grid.GetGridValueOrDefault(x, y);
				if (t.HasRoadDeadEnd())
				{
					potentialSpawnPoints.Add(t);
				}
			}
		}
		// GD.Print($"SpawnPoints: {potentialSpawnPoints}");

		GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid, 
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

		// Spawn enemies
		List<Enemy> testEnemies = [];
		for (int i = 0; i < 5; i++)
		{
			var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
			enemy.movementSpeed = 25.0f;
			testEnemies.Add(enemy);
			GetTree().GetRoot().CallDeferred("add_child", enemy);
		}

		// Set enemy paths
		Random randomizer = new();
		for (int i = 0; i < testEnemies.Count; i++)
		{
			var enemy = testEnemies[i];
			var spawnPoint = potentialSpawnPoints[randomizer.Next(potentialSpawnPoints.Count)].position;
			var path = pathfinder.GetPathInPositions(spawnPoint, hubLocation, grid.cellSize);
			enemy.SetPath(path);
			enemy.GlobalPosition = grid.GetCentralGridCellPositionPixels(spawnPoint);
		}

	}

}
