using Godot;
using System;

public partial class TradingPort : GenericStructure
{
    private const int PORT_CONNECTION_RANGE = 3;
    private Inventory _inventory;

    public override void Initialize(StructureStats stats)
    {
        base.Initialize(stats);
        ConnectedPorts.Add(this, PORT_CONNECTION_RANGE);
    }

}
