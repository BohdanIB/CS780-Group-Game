using Godot;

public class GroundTile
{
    public TerrainType terrain;
    public bool[] roadConnections = new bool[4]; // N,E,S,W
    public Vector2I position;

    public GroundTile(TerrainType terrain, Vector2I position, bool[] roads = null)
    {
        roadConnections = roads ?? [false, false, false, false];
        this.terrain = terrain;
        this.position = position;
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
        return $"{terrain} {roadConnections}";
    }

}
