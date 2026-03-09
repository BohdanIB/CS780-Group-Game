using System;
using Godot;

public class GenericGrid<TGridObject>
{
	private int width, height;
	public float cellSize;
	public TGridObject[,] gridArray {get; private set;} // TODO: remove getter

	public GenericGrid(int width, int height, Func<GenericGrid<TGridObject>, int, int, TGridObject> createGridObject, float cellSize = 1) {
		this.width = width;
		this.height = height;
		gridArray = new TGridObject[width, height];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				gridArray[x, y] = createGridObject(this, x, y);
			}
		} 

		this.cellSize = cellSize;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="considerDiagonals"></param>
	/// <returns>List of neighboring values. Starts with 'North' and continues clockwise</returns>
	public TGridObject[] GetNeighbors(int x, int y, bool considerDiagonals = true)
	{
		TGridObject[] outArray = new TGridObject[considerDiagonals ? 8 : 4];

		int assignmentIndex = 0;

		outArray[assignmentIndex++] = GetGridValueOrDefault(x, y-1);
		if (considerDiagonals) outArray[assignmentIndex++] = GetGridValueOrDefault(x+1, y-1);
		outArray[assignmentIndex++] = GetGridValueOrDefault(x+1, y);
		if (considerDiagonals) outArray[assignmentIndex++] = GetGridValueOrDefault(x+1, y+1);
		outArray[assignmentIndex++] = GetGridValueOrDefault(x, y+1);
		if (considerDiagonals) outArray[assignmentIndex++] = GetGridValueOrDefault(x-1, y+1);
		outArray[assignmentIndex++] = GetGridValueOrDefault(x-1, y);
		if (considerDiagonals) outArray[assignmentIndex++] = GetGridValueOrDefault(x-1, y-1);

		return outArray;
	}

	public bool IsOnGrid(int x, int y) {
		return x >= 0 && y >= 0 && x < width && y < height;
	}

	public TGridObject GetGridValueOrDefault(int x, int y) {
		if (!IsOnGrid(x, y)) return default;
		return gridArray[x,y];
	}

	public void SetGridValue(int x, int y, TGridObject newValue) {
		if (!IsOnGrid(x, y)) return;

		gridArray[y, x] = newValue;
	}

	public int GetHeight() {
		return this.height;
	}

	public int GetWidth() {
		return this.width;
	}

	public void ForEach(Action<TGridObject> action) {
		 for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				action(gridArray[x, y]);
			}
		 }
	}

	public void ForEach(Action<TGridObject> action, Vector2I start, Vector2I end) {
		 for (int y = start.Y; y < end.Y; y++) {
			for (int x = start.X; x < end.X; x++) {
				action(gridArray[x, y]);
			}
		 }
	}

	public Vector2I GetGridCoords(Vector2 position) {
		Vector2I coords = new((int) (position.X / cellSize), (int) (position.Y / cellSize));
		return coords;
	}

	public bool IsAreaOnGrid(Vector2I start, Vector2I end) {
		if (!IsOnGrid(start.X, start.Y) || !IsOnGrid(end.X, end.Y)) return false;

		return true;
	}

	public Vector2I GetGridDimensions()
	{
		return new Vector2I(width, height);
	}
	
	public Vector2 GetCentralGridCellPositionPixels(Vector2I pos)
	{
		return new Vector2((pos.X * cellSize) + (cellSize / 2), (pos.Y * cellSize) + (cellSize / 2));
	}
}
