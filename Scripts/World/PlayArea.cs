using Godot;
using System;

public partial class PlayArea : Node
{
    public static PlayArea instance;
    public GenericGrid<GroundTile> Grid {get; private set;}
    [Export] public GridRenderer GridRenderer {get; private set;}

    public override void _Ready()
    {
        //instance ??= this; Changing this to try and get play area to reset on gameover for reloading the world
        instance = this;
    }

    public void Initialize(GenericGrid<GroundTile> grid)
    {
        Grid = grid;
    }

    public void Render()
    {
        GridRenderer.RenderGrid(Grid);
    }

}
