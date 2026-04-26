using Godot;
using System;

public partial class PlayArea : Node
{
    public static PlayArea instance;
    public GenericGrid<GroundTile> Grid {get; private set;}
    [Export] public GridRenderer GridRenderer {get; private set;}

    public override void _Ready()
    {
        instance ??= this;
    }

    public void Initialize(GenericGrid<GroundTile> grid)
    {
        Grid = grid;
    }

    public void Render()
    {
        GridRenderer.RenderGrid(Grid);
    }

      public static void ResetInstance()
    {
        instance = null;
    }

}
