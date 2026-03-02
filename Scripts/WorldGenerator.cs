using Godot;
using System;
using System.Collections.Generic;

public partial class WorldGenerator : Node
{

    public static GenericGrid<GroundTile> GenerateWorldAStar(Vector2I dimensions, Vector2I hubLocation, int seed = 42)
    {
        Random randomizer = new(seed);

        TerrainType defaultTerrain = new TerrainType()
        {
            groundTileAtlasCoords = new Vector2I(1, 0)  
        };
        TerrainType alternateTerrain = new TerrainType()
        {
            groundTileAtlasCoords = new Vector2I(2, 0)  
        };

        float[,] noise = GenerateNoiseMatrix(dimensions.X, dimensions.Y, seed: randomizer.Next());


        GenericGrid<GroundTile> newWorld = new GenericGrid<GroundTile>(dimensions.X, dimensions.Y, (g, x, y) => new GroundTile((noise[x,y] < .5f) ? defaultTerrain : alternateTerrain, new Vector2I(x, y)));

        GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(newWorld, 
                (tile) =>
                {
                    return 1 + (tile.HasRoadConnection() ? 0 : (1 + (4 * noise[tile.position.X, tile.position.Y])));
                    // Distance from hub metric: (-(Math.Abs(tile.position.X - hubLocation.X) + Math.Abs(tile.position.Y - hubLocation.Y)) + dimensions.X+dimensions.Y)
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

        List<Vector2I> targetPoints = [];

        // Draw initial paths
        for (int i = 0; i < 25; i++)
        {
            Vector2I targetPoint = new Vector2I(randomizer.Next(dimensions.X), randomizer.Next(dimensions.Y));
            if (targetPoint.DistanceTo(hubLocation) < 6) continue;
            targetPoints.Add(targetPoint);

            Vector2I currentPoint = hubLocation;
            foreach (Vector2I nextPoint in pathfinder.GetPath(hubLocation, targetPoint)[1..])
            {
                
                Vector2I direction = nextPoint - currentPoint;

                GroundTile currentTile = newWorld.GetGridValueOrDefault(currentPoint.X, currentPoint.Y);
                GroundTile nextTile = newWorld.GetGridValueOrDefault(nextPoint.X, nextPoint.Y);

                currentTile.roadConnections[GetDirectionAsIndex(direction)] = true;
                nextTile.roadConnections[GetDirectionAsIndex(-direction)] = true;

                currentPoint = nextPoint;

            }

            pathfinder.UpdateGrid();
        }

        // Draw potential loops

        for (int i = 0; i < 0; i++)
        {
            Vector2I initialPoint = targetPoints[randomizer.Next(targetPoints.Count)];
            Vector2I finalPoint = targetPoints[randomizer.Next(targetPoints.Count)];

            if (initialPoint == finalPoint || initialPoint.DistanceTo(finalPoint) < 3) continue;

            Vector2I currentPoint = initialPoint;
            foreach (Vector2I nextPoint in pathfinder.GetPath(initialPoint, finalPoint)[1..])
            {
                
                Vector2I direction = nextPoint - currentPoint;

                GroundTile currentTile = newWorld.GetGridValueOrDefault(currentPoint.X, currentPoint.Y);
                GroundTile nextTile = newWorld.GetGridValueOrDefault(nextPoint.X, nextPoint.Y);

                currentTile.roadConnections[GetDirectionAsIndex(direction)] = true;
                nextTile.roadConnections[GetDirectionAsIndex(-direction)] = true;

                currentPoint = nextPoint;

            }

            pathfinder.UpdateGrid();
        }


        return newWorld;
    }

    public static float[,] GenerateNoiseMatrix(int width, int height, int seed = 42, float zoom = 1, bool normalize = true) {
        float minValue = 1, maxValue = -1;
        FastNoiseLite noiseGenerator = new()
        {
            NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            Frequency = 0.06f,
            FractalOctaves = 6,
            FractalLacunarity = 1.5f,
            Seed = seed
        };



        

        float[,] output = new float[width, height];



        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                output[x, y] = noiseGenerator.GetNoise2D(x*zoom, y*zoom);
                minValue = Mathf.Min(minValue, output[x, y]);
                maxValue = Mathf.Max(maxValue, output[x, y]);
            }

        }

        if (normalize)
        {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++)
                {
                    output[x, y] = Mathf.InverseLerp(minValue, maxValue, output[x, y]);
                }
            }
        }
        
        return output;
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
}
