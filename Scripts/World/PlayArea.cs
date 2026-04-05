using Godot;
using System;

public partial class PlayArea : Node
{
    public static PlayArea instance;
    public GenericGrid<GroundTile> grid;
    [Export] private GridRenderer gridRenderer;

    public override void _Ready()
    {
        instance ??= this;
    }

    public void Render()
    {
        gridRenderer.RenderGrid(grid);
    }

}
