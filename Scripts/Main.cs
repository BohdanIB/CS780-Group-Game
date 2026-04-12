using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    [Export] public ulong MAIN_SEED = 12345;

    [Export] private GridRenderer _gridRenderer;
    [Export] private TurretPlacer _turretPlacer;

    public override void _Ready()
    {
     
        Random randomizer = new Random((int)MAIN_SEED);

        GD.Seed(MAIN_SEED);

        var hubLocation = new Vector2I(20, 10);
        GenericGrid<GroundTile> grid =
            WorldGenerator.GenerateWorldAStar(new Vector2I(41, 21), hubLocation);

        _gridRenderer.RenderGrid(grid);

        var tileSize = _gridRenderer.TerrainMap.TileSet.TileSize;
        var mapWidth = grid.GetWidth() * tileSize.X;
        var mapHeight = grid.GetHeight() * tileSize.Y;

        float zoomX = 1920f / mapWidth;
        float zoomY = 1080f / mapHeight;
        float zoom = Mathf.Min(zoomX, zoomY);

        GD.Print($"zoomX: {zoomX}, zoomY: {zoomY}");

        var center = _gridRenderer.Position +
                     new Vector2(mapWidth / 2f, mapHeight / 2f);

        Camera2D camera = GetNode<Camera2D>("Camera2D");
        camera.Position = center;
        camera.Zoom = new Vector2(zoom, zoom);

       
        _turretPlacer.Initialize(grid, _gridRenderer.TerrainMap);

       
        EnemySpawner spawner = GetNode<EnemySpawner>("EnemySpawner");
        spawner.Initialize(grid, hubLocation, randomizer);

        
        Friendly.TempFriendlyDemo(this, grid, hubLocation, randomizer);
    }
}
