using Godot;
using System;

public partial class TradingPort : GenericStructure
{
    private const int PORT_CONNECTION_RANGE = 3;

    public override void Initialize(StructureStats stats)
    {
        base.Initialize(stats);
        PortConnectionProximity = PORT_CONNECTION_RANGE;
    }

}
