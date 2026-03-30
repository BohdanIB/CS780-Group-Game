
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public int MAIN_SEED = 12345;

	private Random _random;
	Vector2I _hubLocation = new Vector2I(20, 10);
	GenericGrid<GroundTile> _grid;

	public override void _Ready()
	{
		_random = new(MAIN_SEED); // todo: might just want a global seeded randomizer singleton kicking around somewhere?

		_grid = WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), _hubLocation, MAIN_SEED);

		GridRenderer gridRenderer = GetNode<GridRenderer>("GridRenderer");
		gridRenderer.RenderGrid(_grid);

		TurretPlacer turretPlacer = GetNode<TurretPlacer>("TurretPlacer");
		turretPlacer.Initialize(_grid);
	}

	private bool enemyDemo = true;
	public override void _Process(double delta)
	{
		if (enemyDemo)
		{
			enemyDemo = false;
			var enemies = EnemyDemo();
			// foreach (var e in enemies)
			// {
			// 	e.StartMoving();
			// }
		}
	}


	private List<Enemy> EnemyDemo()
	{
		GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(_grid, 
			(x,y) => {
				List<Vector2I> neighborPositions = [];
				if (_grid.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); // UP
				if (_grid.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); // RIGHT
				if (_grid.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); // DOWN
				if (_grid.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); // LEFT

				const float ROAD_COST = 0f;
				Dictionary<Vector2I, float> neighborCosts = [];

				GroundTile currentTile = _grid.GetGridValueOrDefault(x, y);
				foreach (Vector2I coordinate in neighborPositions)
				{
					GroundTile nextTile = _grid.GetGridValueOrDefault(coordinate.X, coordinate.Y);
					neighborCosts.Add(coordinate, currentTile.HasRoadConnection(nextTile.position - currentTile.position) ? ROAD_COST : int.MaxValue);
				}

				return neighborCosts;
			}
		);

		List<GroundTile> potentialEnemySpawnPoints = [];
		for (int x = 0; x < _grid.GetWidth(); x++)
		{
			for (int y = 0; y < _grid.GetHeight(); y++)
			{
				GroundTile t = _grid.GetGridValueOrDefault(x, y);
				if (t.HasRoadDeadEnd())
				{
					potentialEnemySpawnPoints.Add(t);
				}
			}
		}

		List<Enemy> testEnemies = [];
		for (int i = 0; i < 1; i++)
		{
			var enemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate<Enemy>();
			testEnemies.Add(enemy);
			GetTree().GetRoot().AddChild(enemy);

			// Set enemy paths
			var spawnPoint = potentialEnemySpawnPoints[_random.Next(potentialEnemySpawnPoints.Count)].position;
			var path = pathfinder.GetPathInPositions(spawnPoint, _hubLocation, _grid.cellSize);
			enemy.GlobalPosition = _grid.GetCentralGridCellPositionPixels(spawnPoint);
			enemy.Initialize(i == 0 ? EnemyStats.Category.Strong : EnemyStats.Category.Regular, path: path.ToArray()); // Make one 'strong' enemy for testing
			enemy.StartMoving();

			// GD.Print($"{enemy}");
		}
		return testEnemies;
	}

}
