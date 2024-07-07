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

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 1) * cellSize + originPosition;
        }

        public Vector2 GetWorldCoordinates(Vector3 worldPosition)
        {
            Vector2 worldCoords = new Vector2();
            worldCoords.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            worldCoords.y = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            return worldCoords;
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
            Vector2 worldCoords = GetWorldCoordinates(worldPosition);
            SetGridObject((int)worldCoords.x, (int)worldCoords.y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x < 0 || y < 0 || x > width || y > height)
            {
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
            Vector2 worldCoords = GetWorldCoordinates(worldPosition);
            return GetGridObject((int)worldCoords.x, (int)worldCoords.y);
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

        public void DrawDebug()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100.0f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100.0f);
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100.0f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100.0f);
            }
        }
    }
}