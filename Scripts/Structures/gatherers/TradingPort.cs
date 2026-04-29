using Godot;
using System;

public partial class TradingPort : GenericStructure
{
    private const int PORT_CONNECTION_RANGE = 3;
    public Inventory StorageInventory {get; private set;}

    public override void Initialize(StructureStats stats)
    {
        base.Initialize(stats);
        ClosestPortProximity = PORT_CONNECTION_RANGE;
        ConnectedPort = this;

        StorageInventory = new();
    }
}
