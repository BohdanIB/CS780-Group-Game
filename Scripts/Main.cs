using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		PathFollower pf = GetNode<PathFollower>("PathFollower");

		Vector2I startingPosition = new Vector2I(0, 0);
		Vector2I endingPosition = new Vector2I(10, 10);
		GridRenderer gr = GetNode<GridRenderer>("GridRenderer");
		GenericGrid<GroundTile> grid = new(11, 11, (g, x, y) => null, cellSize: 16);
		grid.SetGridValue(startingPosition.X, startingPosition.Y, new GroundTile(new Vector2I(2, 0), [true, true, false, false]));
		grid.SetGridValue(endingPosition.X, endingPosition.Y, new GroundTile(new Vector2I(2, 0), [false, false, true, true]));


		gr.RenderGrid(grid);
		GridAStarPathfinder<GroundTile> pathfinder = new GridAStarPathfinder<GroundTile>(grid, (tile) => tile == null ? -1 : 1, (x, y) =>
		{
			GroundTile tile = grid.GetGridValueOrDefault(x, y);
			if (!tile.HasRoadConnection()) return [];
			List<Vector2I> neighborCoordinates = [];
			if (tile.HasRoadConnection(Vector2I.Up)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Up);
			if (tile.HasRoadConnection(Vector2I.Right)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Right);
			if (tile.HasRoadConnection(Vector2I.Down)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Down);
			if (tile.HasRoadConnection(Vector2I.Left)) neighborCoordinates.Add(new Vector2I(x, y)+Vector2I.Left);

			return [.. neighborCoordinates];
		});
		pathfinder.UpdateGrid();

		GenericGrid<GroundTile> shapeGrid = new GenericGrid<GroundTile>(4, 2, (g, x, y) => null, cellSize: 16);
		shapeGrid.SetGridValue(0, 0, new GroundTile(new Vector2I(1, 0), roads:[false, true, false, true]));
		shapeGrid.SetGridValue(1, 0, new GroundTile(new Vector2I(1, 0), roads:[true, true, true, true]));
		shapeGrid.SetGridValue(2, 0, new GroundTile(new Vector2I(1, 0), roads:[false, true, false, true]));
		shapeGrid.SetGridValue(3, 0, new GroundTile(new Vector2I(1, 0), roads:[true, true, true, true]));
		shapeGrid.SetGridValue(1, 1, new GroundTile(new Vector2I(1, 0), roads:[true, false, true, false]));
		Shape shape = new(shapeGrid);

		ShapePlacer placer = GetNode<ShapePlacer>("ShapePlacer");
		placer.Initialize(shape, grid);

		placer.OnShapePlaced += () => 
			{
				gr.RenderGrid(grid); 
				pathfinder.UpdateGrid();
				List<Vector2> path = pathfinder.GetPathInPositions(startingPosition, endingPosition, 16);
				if (path != null)
				{
					foreach (Vector2 node in path)
					{
						GD.Print($" - {node}");
					}
					pf.SetPath(path);
				}
			};
	}

}
