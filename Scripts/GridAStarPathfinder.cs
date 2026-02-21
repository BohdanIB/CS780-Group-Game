using Godot;
using System;
using System.Collections.Generic;

public class GridAStarPathfinder<TGridObject>
{
    private GenericGrid<TGridObject> terrainGrid;
    private Func<TGridObject, int> traversibilityFunction;
    private Func<int, int, Vector2I[]> neighborFunction;

    private GenericGrid<PathfindingNode> nodeGrid;

    public GridAStarPathfinder(GenericGrid<TGridObject> terrainGrid, Func<TGridObject, int> traversibilityFunction, Func<int, int, Vector2I[]> neighborFunction)
    {
        this.terrainGrid = terrainGrid;
        this.traversibilityFunction = traversibilityFunction;
        this.neighborFunction = neighborFunction;

        UpdateGrid();
    }
    public void UpdateGrid()
    {
        nodeGrid = new GenericGrid<PathfindingNode>(terrainGrid.GetWidth(), terrainGrid.GetHeight(), (g, x, y) =>
        {
            int traversalCost = traversibilityFunction(terrainGrid.GetGridValueOrDefault(x, y));
            if (traversalCost < 0) return null;
            
            PathfindingNode newNode = new(x, y, neighborFunction(x, y));
            newNode.traversalCost = traversalCost;

            return newNode;

        });
    }

    public List<Vector2I> GetPath(Vector2I origin, Vector2I destination)
    {
        if (!nodeGrid.IsOnGrid(origin.X, origin.Y) || !nodeGrid.IsOnGrid(destination.X, destination.Y)) return null;

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
            foreach (Vector2I neighborCoordinate in currentNode.neighborCoordinates)
            {
                PathfindingNode neighborNode = nodeGrid.GetGridValueOrDefault(neighborCoordinate.X, neighborCoordinate.Y);
                if (neighborNode == null)
                {
                    continue;
                }
                if (!neighborNode.IsTraversible())
                {
                    continue;
                }
                    
                int tentative_gScore = currentNode.gCost + neighborNode.traversalCost;
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
        public int traversalCost = -1;
        public int x, y;
        public int gCost = int.MaxValue, hCost;
        public PathfindingNode previousNode;
        public Vector2I[] neighborCoordinates;

        public int GetFCost()
        {
            return gCost + hCost;
        }

        public PathfindingNode(int x, int y, Vector2I[] neighborCoordinates)
        {
            this.x = x;
            this.y = y;
            this.neighborCoordinates = neighborCoordinates;
        }

        public bool IsTraversible()
        {
            return traversalCost >= 0;
        }
    }
}
