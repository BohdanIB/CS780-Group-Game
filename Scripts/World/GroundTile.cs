using Godot;

public class GroundTile
{
    // Tile features
    public BiomeType terrain;
    public Vector2I position;
    public bool[] roadConnections = new bool[4]; // N,E,S,W
    private GenericStructure _structure;

    // Getters + Setters
    public GenericStructure Structure { get => _structure; set => _structure = value; }
    public Turret Turret { get => _structure as Turret; set => _structure = value; }

    public GroundTile(BiomeType terrain, Vector2I position, bool[] roads = null, Turret turret = null)
    {
        this.terrain = terrain;
        this.position = position;
        roadConnections = roads ?? [false, false, false, false];
        _structure = turret;
    }

    // public Vector2I GetGridPosition()
    // {
    //     return position;
    // }

    // public Vector2 GetPixelPosition()
    // {
    //     return new Vector2(position.X * )
    // }

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

    /// <summary>
    /// TODO: Temporary dead end check function. Checks if this tile is a dead-end.
    /// </summary>
    /// <returns></returns>
    public bool HasRoadDeadEnd()
    {
        return (roadConnections[0] && !roadConnections[1] && !roadConnections[2] && !roadConnections[3]) ||
               (!roadConnections[0] && roadConnections[1] && !roadConnections[2] && !roadConnections[3]) ||
               (!roadConnections[0] && !roadConnections[1] && roadConnections[2] && !roadConnections[3]) ||
               (!roadConnections[0] && !roadConnections[1] && !roadConnections[2] && roadConnections[3]);
    }

    public bool HasStructure()
    {
        return _structure != null;
    }

    public override string ToString()
    {
        return $"{terrain} {position} {roadConnections} {(HasStructure() ? _structure : "No Structure")}";
    }

}
