using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");



		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorldAStar(new Vector2I(66, 40), new Vector2I(33, 20), seed:19);
		gr.RenderGrid(grid);
	}

}
