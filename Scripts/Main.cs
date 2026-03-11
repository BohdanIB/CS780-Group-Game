using System;
using System.Collections.Generic;
using Godot;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		var hubLocation = new Vector2I(10, 10);
		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(66, 41), hubLocation, seed:118);

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
			(x,y) => {
					List<Vector2I> neighborPositions = [];
                    if (grid.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); // UP
                    if (grid.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); // RIGHT
                    if (grid.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); // DOWN
                    if (grid.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); // LEFT

                    Dictionary<Vector2I, float> neighborCosts = [];

                    GroundTile currentTile = grid.GetGridValueOrDefault(x, y);
                    

                    foreach (Vector2I coordinate in neighborPositions)
                    {
                        GroundTile nextTile = grid.GetGridValueOrDefault(coordinate.X, coordinate.Y);


                        if (currentTile.HasRoadConnection(nextTile.position - currentTile.position))
                        {
							neighborCosts.Add(coordinate, 1);
                        } 

                        
                    }

                    return neighborCosts;
			}
		);

		// Spawn enemies
		List<Enemy> testEnemies = [];
		for (int i = 0; i < 5; i++)
		{
			var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
			enemy.Initialize(100.0f, 25.0f);
			testEnemies.Add(enemy);
			GetTree().GetRoot().CallDeferred("add_child", enemy); // Cannot add children in _Ready()
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

		// GD.Print($"Enemy {testEnemies[0].Name} distance to hub: {testEnemies[0].GetDistanceToGoalPixels()}");

	}

}
