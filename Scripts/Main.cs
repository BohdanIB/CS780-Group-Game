using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");



		GenericGrid<GroundTile> grid = WorldGenerator.GenerateWorld(new Vector2I(21, 21), new Vector2I(10, 10));
		gr.RenderGrid(grid);

		// GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid, (tile) => tile == null ? -1 : 1, (x, y) =>
		// {
		// 	GroundTile tile = grid.GetGridValueOrDefault(x, y);
		// 	if (!tile.HasRoadConnection()) return [];
		// 	List<Vector2I> neighborCoordinates = [];
		// 	if (tile.HasRoadConnection(Vector2I.Up)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Up);
		// 	if (tile.HasRoadConnection(Vector2I.Right)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Right);
		// 	if (tile.HasRoadConnection(Vector2I.Down)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Down);
		// 	if (tile.HasRoadConnection(Vector2I.Left)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Left);

		// 	return [.. neighborCoordinates];
		// });
	}

}
