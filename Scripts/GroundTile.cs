using Godot;

public class GroundTile
{
    // Tile features
    public TerrainType terrain;
    public Vector2I position;
    public bool[] roadConnections = new bool[4]; // N,E,S,W
    private Turret _turret = null;

    public Turret Turret { get => _turret; set => _turret = value; }

    public GroundTile(TerrainType terrain, Vector2I position, bool[] roads = null, Turret turret = null)
    {
        this.terrain = terrain;
        this.position = position;
        roadConnections = roads ?? [false, false, false, false];
        Turret = turret;
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

    public bool HasTurret()
    {
        return _turret != null;
    }

    public override string ToString()
    {
        return $"{terrain} {position} {roadConnections} {(HasTurret() ? Turret : "No Turret")}";
    }

}
