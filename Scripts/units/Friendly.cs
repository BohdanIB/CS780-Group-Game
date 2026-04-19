using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;

public partial class Friendly : PathFollower
{
    [Export] private FriendlyStats _stats;

    [ExportGroup("Types")]
    [Export] public Groups.GroupTypes _friendlyTypes = PathFollower.TYPES | Groups.GroupTypes.Friendly;
    [Export] public Groups.GroupTypes _enemyTypes = Groups.GroupTypes.Enemy;

    public void Initialize(FriendlyStats stats, Vector2[] path = null)
    {
        _stats = stats;

        InitializeComponents();
        UpdateStats();

        if (path != null)
        {
            SetPath(path);
            StartMoving();
        }
    }

    public override void _Ready()
    {
        base._Ready();

        // Health + damage
        _health.OnNoHealthLeft += () =>
        {
            GD.Print($"Friendly {Name} died.");
            QueueFree();
        };

        _hurt.OnHurt += (area, damage) =>
        {
            _health.ApplyDamage(damage);
        };

        // Movement direction updates
        _mover.Connect(
            MoverComponent.SignalName.OnPathPointReached,
            new Callable(this, nameof(OnPathPointReached))
        );
    }

    private void OnPathPointReached(bool hasNextPoint, Vector2 nextPoint)
    {
        if (hasNextPoint)
        {
            float angle = GlobalPosition.AngleToPoint(nextPoint);
            _animation.SetDirection(angle);
        }
    }

    public void UpdateStats(FriendlyStats newStats = null)
    {
        if (newStats != null)
            _stats = newStats;

        UpdateComponents();
    }

    private void InitializeComponents()
    {
        if (_stats != null)
        {
            _health.SetHealth(_stats.Health);
            _hurt.Initialize(_friendlyTypes, _enemyTypes);
            _detector.Initialize(_friendlyTypes, _enemyTypes);
            _detectable.Initialize(_friendlyTypes, _enemyTypes);
            _mover.Initialize(_stats.MovementSpeed, this, start: false);
            _animation.Initialize(_stats.Animations);
        }
    }

    public void UpdateComponents()
    {
        if (_stats != null)
        {
            _health.SetHealth(_stats.Health);
            _hurt.SetRadius(_stats.HitboxRadius);
            _detector.SetRadius(_stats.AggroRadius);
            _detectable.SetRadius(_stats.DetectableRadius);
            _mover.Speed = _stats.MovementSpeed;
            _animation.Animations = _stats.Animations;
        }
    }

    public override string ToString()
    {
        return $"Friendly '{Name}': {_stats}";
    }

    public static void TempFriendlyDemo(Node parent, GenericGrid<GroundTile> grid, IsometricTileMap tileMap, Vector2I hub)
    {
        GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid,
            (x, y) =>
            {
                List<Vector2I> neighborPositions = [];
                if (grid.IsOnGrid(x, y - 1)) neighborPositions.Add(new Vector2I(x, y - 1));
                if (grid.IsOnGrid(x + 1, y)) neighborPositions.Add(new Vector2I(x + 1, y));
                if (grid.IsOnGrid(x, y + 1)) neighborPositions.Add(new Vector2I(x, y + 1));
                if (grid.IsOnGrid(x - 1, y)) neighborPositions.Add(new Vector2I(x - 1, y));

                const float ROAD_COST = 0f;
                Dictionary<Vector2I, float> neighborCosts = [];

                GroundTile currentTile = grid.GetGridValueOrDefault(x, y);
                foreach (Vector2I coordinate in neighborPositions)
                {
                    GroundTile nextTile = grid.GetGridValueOrDefault(coordinate.X, coordinate.Y);
                    neighborCosts.Add(coordinate,
                        currentTile.HasRoadConnection(nextTile.position - currentTile.position)
                            ? ROAD_COST
                            : int.MaxValue);
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
                    potentialFriendlyEndpoints.Add(t);
            }
        }

        var layers = tileMap.GetLayers();
        if (layers.Length <= 0)
        {
            GD.Print("WARNING: COULD NOT RUN FRIENDLY TEMP DEMO - NO LAYERS IN TILE MAP!");
            return;
        }

        var layer = layers[0];

        for (int i = 0; i < 3; i++)
        {
            var friendly = GD.Load<PackedScene>("res://Scenes/friendly.tscn").Instantiate<Friendly>();
            parent.CallDeferred("add_child", friendly);

            foreach (var stats in FriendlyStats.ALL_FRIENDLIES)
            {
                if (stats.Type == FriendlyStats.Category.Regular)
                {
                    friendly.Initialize(stats);
                    break;
                }
            }

            var endPoint = potentialFriendlyEndpoints[GD.RandRange(0, potentialFriendlyEndpoints.Count - 1)].position;

            var path = new List<Vector2>();
            foreach (var point in pathfinder.GetPath(hub, endPoint))
                path.Add(IsometricTileMap.MapCoordToGlobalPosition(layer, point));

            friendly.SetPath(path.ToArray());
            friendly.StartMoving();
            friendly.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(layer, hub);
        }
    }
}
