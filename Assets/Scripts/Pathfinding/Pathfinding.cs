﻿/*
    --------------------------------------------------
    This script was built on top of CodeMonkey's Pathfinding tutorial.
    Check his version at: unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    // Move cost consts
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    // Singleton
    public static Pathfinding Instance { get; private set; }

    // Private fields
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    /// <summary>
    /// Default constructor
    /// Creates a new instance of the pathfinding class
    /// Creates a new grid and sets the instance to this class
    /// </summary>
    /// <param name="grid"></param>
    public Pathfinding(Grid<GridObject> grid, Transform pathfindingParent) {
        Instance = this;
        this.grid = new Grid<PathNode>(grid.GetWidth(), grid.GetHeight(), grid.GetCellSize(), grid.GetOriginPosition(), pathfindingParent, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    /// <summary>
    /// Grid getter
    /// </summary>
    /// <returns> Grid </returns>
    public Grid<PathNode> GetGrid() {
        return grid;
    }

    /// <summary>
    /// Finds the shortest path to the given destination with A*
    /// If found returns a list of PathNodes from the start to the end point
    /// Else return null
    /// </summary>
    /// <param name="startWorldPosition"></param>
    /// <param name="endWorldPosition"></param>
    /// <returns> List of PathNode objects </returns>
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path) {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    /// <summary>
    /// Finds the shortest path to the given destination with A*
    /// If found returns a list of PathNodes from the start to the end point
    /// Else return null
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns> List of PathNode objects </returns>
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null) {
            // Invalid Path
            return null;
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();
        

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.CanBuild()) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    /// <summary>
    /// Gets the neighbour nodes of the given node, and returns them in a list
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns> List of neighbour nodes </returns>
    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    /// <summary>
    /// Finds the PathNode at the given x and y coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns> PathNode at the desired coordinates </returns>
    public PathNode GetNode(int x, int y) {
        return grid.GetGridObject(x, y);
    }

    /// <summary>
    /// Finds the path to follow from the end node to the start node
    /// Returns a list of PathNodes to the path
    /// ONLY CALLED WHEN THE PATH FINDER HAS FOUND A PATH
    /// </summary>
    /// <param name="endNode"></param>
    /// <returns> List of pathnodes to the target</returns>
    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Calculates the distance cost between two nodes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns> Cost value as int </returns>
    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    /// <summary>
    /// Finds the node with the lowest fCost in the given list
    /// </summary>
    /// <param name="pathNodeList"></param>
    /// <returns> Node with the lowest f cost </returns>
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

}
