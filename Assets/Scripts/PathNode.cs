using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame;
public class PathNode
{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public Vector3 GetWorldCoords() 
    { 
        Vector3 worldCoords = grid.GetWorldPosition(x,y);
        return worldCoords; 
    }
}
