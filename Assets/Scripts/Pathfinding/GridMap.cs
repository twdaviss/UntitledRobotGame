using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame;

public class GridMap : MonoBehaviour
{
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private Grid<PathNode> grid;

    private void Awake()
    {
        grid = new Grid<PathNode>(width, height, cellSize, transform.position, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        InitializeGrid();
    }

    private void Update()
    {
        if (enableDebug) { grid.DrawDebug(); }  
    }

    public void InitializeGrid()
    {
        int layerMask = LayerMask.GetMask("Obstacles") | LayerMask.GetMask("Grapple");
        foreach (PathNode node in grid.GetAllGridObjects())
        {
            Vector2 point = new Vector2(node.GetWorldCoords().x + grid.GetCellSize() / 2, node.GetWorldCoords().y + grid.GetCellSize() / 2);
            if (Physics2D.OverlapCircle(point, grid.GetCellSize() / 3,layerMask))
            {
                node.isWalkable = false;
            }
        }
    }

    public Grid<PathNode> GetGrid() { return grid; }

    private void OnDrawGizmos()
    {
        if (grid == null || !enableDebug)
        {
            return;
        }
        foreach (PathNode node in grid.GetAllGridObjects())
        {
            Vector3 pos = node.GetWorldCoords();
            pos.x += cellSize / 2;
            pos.y += cellSize / 2;

            if (node.isWalkable)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(pos, new Vector3(grid.GetCellSize(), grid.GetCellSize(), grid.GetCellSize()));
            }
            else
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(pos, new Vector3(grid.GetCellSize(), grid.GetCellSize(), grid.GetCellSize()));
            }
        }
    }
}
