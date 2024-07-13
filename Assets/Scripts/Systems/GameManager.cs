using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private Pathfinding pathfinder;

    private Grid<PathNode> currentGrid;
    public Grid<PathNode> ActiveGrid { get { return currentGrid; } }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        SetGrid();
        Instance = this;
        return;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChange;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        SetGrid();
    }

    private void SetGrid()
    {
        GridMap[] gridMaps = FindObjectsByType<GridMap>(FindObjectsSortMode.None);
        if (gridMaps.Length > 1)
        {
            Debug.Assert(gridMaps.Length > 0, "Too many gridmaps in scene");
        }
        else
        {
            currentGrid = gridMaps[0].GetGrid();
        }
        pathfinder = new Pathfinding(currentGrid);
    }

}
