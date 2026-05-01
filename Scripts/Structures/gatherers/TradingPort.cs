using Godot;
using System;

public partial class TradingPort : GenericStructure
{
    private const int PORT_CONNECTION_RANGE = 3;
    public Inventory StorageInventory {get; private set;}
    public Vector2I WaterAccessPoint;

    public override void Initialize(StructureStats stats, GroundTile tile)
    {
        base.Initialize(stats, tile);
        ClosestPortProximity = PORT_CONNECTION_RANGE;
        ConnectedPort = this;

        StorageInventory = new();

        FriendlySpawner.Instance.RegisterPort(this);
    }
}
