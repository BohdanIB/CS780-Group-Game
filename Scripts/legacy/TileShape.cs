using Godot;
using System;

public class TileShape
{
    public GenericGrid<Tile> grid;


    public TileShape GetRotatedShape(int direction)
    {
        if (direction == 0) return this;
        GenericGrid<Tile> newGrid = new GenericGrid<Tile>(grid.GetHeight(), grid.GetWidth(), (g, x, y) =>
        {
            Tile tileToCopy = null;
            if (direction > 0)
            {
                tileToCopy = grid.GetGridValueOrDefault(grid.GetWidth() - y - 1, x);
            } 
            else if (direction < 0)
            {
                tileToCopy = grid.GetGridValueOrDefault(y, grid.GetWidth() - x - 1);
            }

            if (tileToCopy == null) return null;

            bool[] newRoadConnections;

            if (tileToCopy.HasRoadConnection())
            {
                newRoadConnections = new bool[4];

                if (direction > 0)
                {
                    newRoadConnections = [tileToCopy.HasRoadConnection(Vector2I.Left), 
                                          tileToCopy.HasRoadConnection(Vector2I.Up), 
                                          tileToCopy.HasRoadConnection(Vector2I.Right), 
                                          tileToCopy.HasRoadConnection(Vector2I.Down)];
                } 
                else if (direction < 0)
                {
                    newRoadConnections = [tileToCopy.HasRoadConnection(Vector2I.Right), 
                                          tileToCopy.HasRoadConnection(Vector2I.Down), 
                                          tileToCopy.HasRoadConnection(Vector2I.Left), 
                                          tileToCopy.HasRoadConnection(Vector2I.Up)];
                }
            } 
            else
            {
                newRoadConnections = [false, false, false, false];
            }

            Tile newTile = new Tile(tileToCopy.tileAtlasCoords);
            newTile.roadConnections = newRoadConnections;

            return newTile;
        });

        TileShape newShape = new(newGrid);

        return newShape;
    }

    public TileShape(GenericGrid<Tile> grid)
    {
        this.grid = grid;
        grid.ForEach((tile) =>
        {
            if (tile != null)
            {
                tile.parentShape = this;
            } 
        });
    }
}
