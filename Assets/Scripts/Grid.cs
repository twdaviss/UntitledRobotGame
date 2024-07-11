using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotGame
{
    public class Grid<TGridObject>
    {
        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;
        private TGridObject[,] gridArray;
        public bool debugEnabled = false;
        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0) * cellSize + originPosition;
        }

        public Vector3 GetGridCoordinates(Vector3 worldPosition)
        {
            Vector3 gridCoords = new Vector3();
            gridCoords.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            gridCoords.y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
            return gridCoords;
        }

        private void SetGridObject(int x, int y, TGridObject value)
        {
            if (x < 0 || y < 0 || x > width || y > height)
            {
                return;
            }

            gridArray[x, y] = value;
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            Vector2 gridCoords = GetGridCoordinates(worldPosition);
            SetGridObject((int)gridCoords.x, (int)gridCoords.y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                Debug.Log("Target is outside bounds of grid map");
                return default;
            }
            return gridArray[x, y];
        }

        public TGridObject[,] GetAllGridObjects()
        {
            return gridArray;
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            Vector2 gridCoords = GetGridCoordinates(worldPosition);
            return GetGridObject((int)gridCoords.x, (int)gridCoords.y);
        }

        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        public float GetCellSize()
        {
            return cellSize;
        }

        public Vector2 GetOrigin()
        {
            return originPosition;
        }
        public void DrawDebug()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                Debug.DrawLine(GetWorldPosition(x, 0), GetWorldPosition(x, height), Color.white, 100.0f);

                //for (int y = 0; y < gridArray.GetLength(1); y++)
                //{
                //    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100.0f);
                //    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100.0f);
                //}
                //Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100.0f);
                //Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100.0f);
            }
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(0, y), GetWorldPosition(width, y), Color.white, 100.0f);
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100.0f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100.0f);
        }
    }
}