using Godot;
using System;
using System.Collections.Generic;

public class GridAStarPathfinder<TGridObject>
{
	private GenericGrid<TGridObject> terrainGrid;

	Func<int, int, Dictionary<Vector2I, float>> neighborFunction;

	private GenericGrid<PathfindingNode> nodeGrid;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="terrainGrid">The grid that paths are navigating through</param>
	/// <param name="neighborFunction">Defines the valid neighbors of a cell and the traversal cost of reaching each one</param>

	public GridAStarPathfinder(GenericGrid<TGridObject> terrainGrid, Func<int, int, Dictionary<Vector2I, float>> neighborFunction)
	{
		this.terrainGrid = terrainGrid;
		this.neighborFunction = neighborFunction;

		UpdateGrid();
	}
	public void UpdateGrid()
	{
		nodeGrid = new GenericGrid<PathfindingNode>(terrainGrid.GetWidth(), terrainGrid.GetHeight(), (g, x, y) =>
		{
			
			PathfindingNode newNode = new(x, y, neighborFunction(x, y));

			return newNode;

		});
	}

	// public List<Vector2> GetPathInPositions(Vector2I origin, Vector2I destination, float cellSize)
	// {
	//     List<Vector2I> path = GetPath(origin, destination);
	//     if (path == null) return null;

	//     List<Vector2> outputPath = new();
	//     foreach(Vector2I coordinate in path)
	//     {
	//         outputPath.Add(((Vector2) coordinate + Vector2.One*0.5f) * cellSize);
	//     }

	//     return outputPath;
	// }

	public List<Vector2I> GetPath(Vector2I origin, Vector2I destination)
	{
		if (!nodeGrid.IsOnGrid(origin.X, origin.Y) || !nodeGrid.IsOnGrid(destination.X, destination.Y)) return null;

		nodeGrid.ForEach((node) => node.Reset());

		PathfindingNode startNode = nodeGrid.GetGridValueOrDefault(origin.X, origin.Y);
		PathfindingNode endNode = nodeGrid.GetGridValueOrDefault(destination.X, destination.Y);

		List<PathfindingNode> openList = new List<PathfindingNode>
		{
			startNode
		};

		startNode.gCost = 0;
		startNode.hCost = ManhattanHeuristic(startNode, endNode);


		while (openList.Count > 0)
		{
			PathfindingNode currentNode = openList[0];
			foreach (PathfindingNode potentialNode in openList)
			{
				if (potentialNode.GetFCost() < currentNode.GetFCost()) currentNode = potentialNode;
			}

			if (currentNode == endNode)
			{
				List<Vector2I> path = new List<Vector2I>();

				PathfindingNode sentinel = endNode;
				while (sentinel != null)
				{
					path.Insert(0, new Vector2I(sentinel.x, sentinel.y));
					sentinel = sentinel.previousNode;
				}

				return path;
			}

			openList.Remove(currentNode);
			foreach (Vector2I neighborCoordinate in currentNode.neighbors.Keys)
			{
				PathfindingNode neighborNode = nodeGrid.GetGridValueOrDefault(neighborCoordinate.X, neighborCoordinate.Y);
				if (neighborNode == null)
				{
					continue;
				}
					
				float tentative_gScore = currentNode.gCost + currentNode.neighbors[neighborCoordinate];
				if (tentative_gScore < neighborNode.gCost)
				{
					neighborNode.previousNode = currentNode;
					neighborNode.gCost = tentative_gScore;
					neighborNode.hCost = ManhattanHeuristic(neighborNode, endNode);
					if (!openList.Contains(neighborNode))
					{
						openList.Add(neighborNode);
					}
				}
			}
		}

		GD.Print(" Exhausted Search Area");
		return null;
	}

	private static int ManhattanHeuristic(PathfindingNode node1, PathfindingNode node2)
	{
		return Math.Abs(node1.x - node2.x) + Math.Abs(node1.y - node2.y);
	}


	private class PathfindingNode
	{
		public int x, y;
		public float gCost = int.MaxValue, hCost;
		public PathfindingNode previousNode;
		public Dictionary<Vector2I, float> neighbors;

		public float GetFCost()
		{
			return gCost + hCost;
		}

		public PathfindingNode(int x, int y, Dictionary<Vector2I, float> neighbors)
		{
			this.x = x;
			this.y = y;
			this.neighbors = neighbors;
		}

		public void Reset()
		{
			gCost = int.MaxValue;
			previousNode = null;
		}
	}
}
