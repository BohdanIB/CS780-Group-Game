
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;

public partial class Friendly : PathFollower
{
	[Export] private FriendlyStats _stats = new(FriendlyStats.Category.Regular);

	[ExportGroup("Types")]
	[Export] public Groups.GroupTypes _friendlyTypes = PathFollower.TYPES | Groups.GroupTypes.Friendly;
	[Export] public Groups.GroupTypes _enemyTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _hurtTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectorTypes = Groups.GroupTypes.None;
	// [Export] public Groups.GroupTypes _detectableTypes = Groups.GroupTypes.Enemy;

	/// <summary>
	/// Initializes friendly with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(FriendlyStats stats, Vector2[] path = null)
	{
		SetPath(path);
		_stats = stats;
		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		base._Ready();

		// Component callbacks //

		_health.OnNoHealthLeft += () =>
		{
			GD.Print($"Friendly {Name} died.");
			QueueFree();
		};
		_hurt.OnHurt += (area, damage) => 
		{
			_health.ApplyDamage(damage); 
		}; 
	}

	public void UpdateStats(FriendlyStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}
	public void UpdateComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_mover.Speed = _stats.MovementSpeed;
			
			// Todo: Add more updates
			UpdateSprite(); // todo: should be a component?
		}
	}
	private void InitializeComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_friendlyTypes, _enemyTypes);
			_detector.Initialize(_friendlyTypes, _enemyTypes);
			_detectable.Initialize(_friendlyTypes, _enemyTypes);
			_mover.Initialize(_stats.MovementSpeed, this, start: true);

			UpdateSprite(); // todo: should be a component?
		}
	}
	private void UpdateSprite()
	{
		_idleAnimations.Frames = _stats.Animations.Idle;
	}
	public override string ToString()
	{
		return $"Friendly '{Name}': {_stats}";
	}

	/// <summary>
	/// TODO - This is a temporary function for testing friendly functionality on the game. This should go away at some point.
	/// 
	/// Spawn friendly units at hub to head towards random tiles with dead-end road segments
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="grid"></param>
	/// <param name="hub"></param>
	public static void TempFriendlyDemo(Node parent, GenericGrid<GroundTile> grid, IsometricTileMap tileMap, Vector2I hub)
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

		var layers = tileMap.GetLayers();
		if (layers.Length <= 0)
		{
			GD.Print("WARNING: COULD NOT RUN FRIENDLY TEMP DEMO - NO LAYERS IN TILE MAP!");
			return;
		}
		var layer = layers[0];
		List<Friendly> testFriendlies = [];
		var allFriendlyStats = FriendlyStats.LoadAllStats();
		for (int i = 0; i < 3; i++)
		{
			var friendly = GD.Load<PackedScene>("res://Scenes/friendly.tscn").Instantiate<Friendly>();

			foreach(var stats in allFriendlyStats)
			{
				if (stats.Type == FriendlyStats.Category.Regular)
				{
					friendly.Initialize(stats);
					break;
				}
			}
			// friendly.Initialize(i == 0 ? FriendlyStats.Category.Loaded : FriendlyStats.Category.Regular); // Make one 'loaded' enemy for testing
			// TODO
			testFriendlies.Add(friendly);
			parent.GetTree().GetRoot().CallDeferred("add_child", friendly); // Cannot add children in _Ready()

			// Set path
			var endPoint = potentialFriendlyEndpoints[GD.RandRange(0, potentialFriendlyEndpoints.Count-1)].position;
			var path = new List<Vector2>();
			foreach (var point in pathfinder.GetPath(hub, endPoint))
			{
				path.Add(IsometricTileMap.MapCoordToGlobalPosition(layer, point));
			}
			friendly.SetPath(path);
			friendly.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(layer, hub);

			// GD.Print($"{friendly}");
		}
		
	}

}
