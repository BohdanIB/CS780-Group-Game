using Godot;
using System;

public partial class TileShapePlacer : Node2D
{

	private TileShape currentShape;
	private GenericGrid<GroundTile> targetGrid;
	private Vector2I currentOriginCoordinates;

	[Signal] public delegate void OnShapePlacedEventHandler();

	public void Initialize(TileShape shapeToPlace, GenericGrid<GroundTile> targetGrid)
	{
		currentShape = shapeToPlace;
		this.targetGrid = targetGrid;

		GetNode<GridRenderer>("GridRenderer").RenderGrid(shapeToPlace.grid);
	}

    public override void _Process(double delta)
    {
    
		if (Input.IsActionJustPressed("Left Click"))
		{
			// Attempt Placement
			if (IsPlacementValid())
			{
				GD.Print("Valid Placement Position");
				PlaceShape();
			}
		}
		if (Input.IsActionJustPressed("Rotate"))
		{
			// Rotate Shape
			currentShape = currentShape.GetRotatedShape(1);
			GetNode<GridRenderer>("GridRenderer").RenderGrid(currentShape.grid);
		}

		FollowMouse();
    }

	private bool IsPlacementValid()
	{
		for (int x = 0; x < currentShape.grid.GetWidth(); x++)
		{
			for (int y = 0; y < currentShape.grid.GetHeight(); y++)
			{
				if (currentShape.grid.GetGridValueOrDefault(x, y) != null)
				{
					if (targetGrid.GetGridValueOrDefault(currentOriginCoordinates.X+x, currentOriginCoordinates.Y+y) != null)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private void FollowMouse()
	{
		Vector2 mousePosition = GetViewport().GetMousePosition();

		currentOriginCoordinates = (Vector2I) (mousePosition / targetGrid.cellSize).Clamp(Vector2I.Zero, targetGrid.GetGridDimensions() - currentShape.grid.GetGridDimensions());

		Position = (Vector2) currentOriginCoordinates * targetGrid.cellSize;
	}

	private void PlaceShape()
	{
		for (int x = 0; x < currentShape.grid.GetWidth(); x++)
		{
			for (int y = 0; y < currentShape.grid.GetHeight(); y++)
			{
				if (targetGrid.GetGridValueOrDefault(currentOriginCoordinates.X+x, currentOriginCoordinates.Y+y) == null) targetGrid.SetGridValue(currentOriginCoordinates.X+x, currentOriginCoordinates.Y+y, currentShape.grid.GetGridValueOrDefault(x, y));
			}
		}
		EmitSignal(nameof(OnShapePlaced));
	}

}
