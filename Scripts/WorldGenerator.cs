using Godot;
using System;
using System.Collections.Generic;

public partial class WorldGenerator : Node
{
    private const float TRAILBLAZER_TERMINATION_CHANCE = 0.03f, TRAILBLAZER_ROTATION_CHANCE = 0.12f, TRAILBLAZER_BRANCH_CHANCE = 0.09f;
    private const int TRAILBLAZER_MIN_COOLDOWN = 2, TRAILBLAZER_MAX_COOLDOWN = 5;

    public static GenericGrid<GroundTile> GenerateWorldAStar(Vector2I dimensions, Vector2I hubLocation, int seed = 42)
    {
        Random randomizer = new();

        TerrainType defaultTerrain = new TerrainType()
        {
            groundTileAtlasCoords = new Vector2I(1, 0)  
        };


        GenericGrid<GroundTile> newWorld = new GenericGrid<GroundTile>(dimensions.X, dimensions.Y, (g, x, y) => new GroundTile(defaultTerrain, new Vector2I(x, y)));

        GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(newWorld, 
                (tile) =>
                {
                    return tile.HasRoadConnection() ? 9 : (-(Math.Abs(tile.position.X - hubLocation.X) + Math.Abs(tile.position.Y - hubLocation.Y)) + dimensions.X+dimensions.Y);
                },
                (x,y) => {
                    List<Vector2I> neighborPositions = [];
                    if (newWorld.IsOnGrid(x, y-1)) neighborPositions.Add(new Vector2I(x, y-1)); 
                    if (newWorld.IsOnGrid(x+1, y)) neighborPositions.Add(new Vector2I(x+1, y)); 
                    if (newWorld.IsOnGrid(x, y+1)) neighborPositions.Add(new Vector2I(x, y+1)); 
                    if (newWorld.IsOnGrid(x-1, y)) neighborPositions.Add(new Vector2I(x-1, y)); 
                    return [.. neighborPositions];
                    }
            );

        for (int i = 0; i < 25; i++)
        {
            Vector2I targetPoint = new Vector2I(randomizer.Next(dimensions.X), randomizer.Next(dimensions.Y));
            if (targetPoint.DistanceTo(hubLocation) < 6) continue;

            Vector2I currentPoint = hubLocation;
            foreach (Vector2I nextPoint in pathfinder.GetPath(hubLocation, targetPoint)[1..])
            {
                
                Vector2I direction = nextPoint - currentPoint;

                GroundTile currentTile = newWorld.GetGridValueOrDefault(currentPoint.X, currentPoint.Y);
                GroundTile nextTile = newWorld.GetGridValueOrDefault(nextPoint.X, nextPoint.Y);

                GD.Print($"Current Position: {currentPoint}   Next Position: {nextPoint}   Heading: {direction} ({GetDirectionAsIndex(direction)})");

                currentTile.roadConnections[GetDirectionAsIndex(direction)] = true;
                nextTile.roadConnections[GetDirectionAsIndex(-direction)] = true;

                currentPoint = nextPoint;

            }

            pathfinder.UpdateGrid();
        }


        return newWorld;
    }
    
    public static GenericGrid<GroundTile> GenerateWorldRandomAgents(Vector2I dimensions, Vector2I hubLocation, int seed = 42)
    {

        Random randomizer = new();

        TerrainType defaultTerrain = new TerrainType()
        {
            groundTileAtlasCoords = new Vector2I(1, 0)  
        };


        GenericGrid<GroundTile> newWorld = new GenericGrid<GroundTile>(dimensions.X, dimensions.Y, (g, x, y) => new GroundTile(defaultTerrain, new Vector2I(x, y)));

        List<TrailBlazer> trailBlazers =
        [
            new TrailBlazer(hubLocation, Vector2I.Up, randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN)),
            new TrailBlazer(hubLocation, Vector2I.Right, randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN)),
            new TrailBlazer(hubLocation, Vector2I.Down, randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN)),
            new TrailBlazer(hubLocation, Vector2I.Left, randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN)),
        ];

        while (trailBlazers.Count > 0)
        {
            TrailBlazer activeTrailBlazer = trailBlazers[randomizer.Next(trailBlazers.Count)];

            GroundTile currentTile = newWorld.GetGridValueOrDefault(activeTrailBlazer.position.X, activeTrailBlazer.position.Y);
            Vector2I nextPosition = activeTrailBlazer.position + activeTrailBlazer.heading;
            GroundTile nextTile = newWorld.GetGridValueOrDefault(nextPosition.X, nextPosition.Y);

            if (currentTile == null || nextTile == null)
            {
                trailBlazers.Remove(activeTrailBlazer);
                continue;
            }

            bool nextTileHadExistingPath = nextTile.HasRoadConnection();

            // GD.Print($"Current Position: {activeTrailBlazer.position}   Next Position: {nextPosition}   Heading: {activeTrailBlazer.heading} ({GetDirectionAsIndex(activeTrailBlazer.heading)})   Inverted Heading: {-activeTrailBlazer.heading} ({GetDirectionAsIndex(-activeTrailBlazer.heading)})");
            currentTile.roadConnections[GetDirectionAsIndex(activeTrailBlazer.heading)] = true;
            nextTile.roadConnections[GetDirectionAsIndex(-activeTrailBlazer.heading)] = true;

            activeTrailBlazer.position = nextPosition;

            if (activeTrailBlazer.cooldown-- > 0) continue;

            if ((nextTileHadExistingPath && randomizer.NextDouble() < .75f) || randomizer.NextDouble() < TRAILBLAZER_TERMINATION_CHANCE)
            {
                trailBlazers.Remove(activeTrailBlazer);
                continue;
            }

            if (randomizer.NextDouble() < TRAILBLAZER_BRANCH_CHANCE)
            {
                int headingIndex = GetDirectionAsIndex(activeTrailBlazer.heading);
                headingIndex += (randomizer.Next(2) == 0) ? 1 : -1;
                if (headingIndex >= 4) headingIndex -=4;
                if (headingIndex < 0) headingIndex += 4;

                trailBlazers.Add(new TrailBlazer(activeTrailBlazer.position, GetDirectionFromIndex(headingIndex), randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN)));
            }

            if (randomizer.NextDouble() < TRAILBLAZER_ROTATION_CHANCE)
            {
                int headingIndex = GetDirectionAsIndex(activeTrailBlazer.heading);
                headingIndex += (randomizer.Next(2) == 0) ? 1 : -1;
                if (headingIndex >= 4) headingIndex -=4;
                if (headingIndex < 0) headingIndex += 4;

                activeTrailBlazer.heading = GetDirectionFromIndex(headingIndex);
                activeTrailBlazer.cooldown = randomizer.Next(TRAILBLAZER_MIN_COOLDOWN, TRAILBLAZER_MAX_COOLDOWN);
            }

        }

        return newWorld;
    }

    private static int GetDirectionAsIndex(Vector2I direction)
    {
        if (direction == Vector2I.Up) return 0;
        if (direction == Vector2I.Right) return 1;
        if (direction == Vector2I.Down) return 2;
        if (direction == Vector2I.Left) return 3;
        return -1;
    }

    private static Vector2I GetDirectionFromIndex(int index)
    {
        return index switch
        {
            0 => Vector2I.Up,
            1 => Vector2I.Right,
            2 => Vector2I.Down,
            3 => Vector2I.Left,
            _ => Vector2I.Zero,
        };

    }

    private class TrailBlazer(Vector2I position, Vector2I heading, int cooldown)
    {
        public Vector2I position = position;
        public Vector2I heading = heading;
        public int cooldown = cooldown;
    }
}
