using System;
using Godot;

public class GroundTile
{
    public TileShape parentShape = null;
    public Vector2I tileAtlasCoords;
    public bool[] roadConnections = new bool[4]; // N,E,S,W

    public GroundTile(Vector2I tileAtlasCoords, bool[] roads = null)
    {
        roadConnections = roads ?? [false, false, false, false];
        this.tileAtlasCoords = tileAtlasCoords;
    }

    public bool HasRoadConnection(Vector2I direction)
    {
        if (direction == Vector2I.Up)
        {
            return roadConnections[0];
        }
        if (direction == Vector2I.Right)
        {
            return roadConnections[1];
        }
        if (direction == Vector2I.Down)
        {
            return roadConnections[2];
        }
        if (direction == Vector2I.Left)
        {
            return roadConnections[3];
        }
        return false;
    }

    public bool HasRoadConnection()
    {
        for (int i = 0; i < 4; i++)
        {
            if (roadConnections[i] == true) return true;
        }
        return false;
    }

    public override string ToString()
    {
        return $"{tileAtlasCoords} {roadConnections}";
    }

}
