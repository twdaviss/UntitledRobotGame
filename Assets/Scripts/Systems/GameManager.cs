using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotGame;
using UnityEngine.SceneManagement;
using System;
public enum GameScene
{
    MainMenu,
    TestScene,
};
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    public static GameManager Instance {  get; private set; }
    private Pathfinding pathfinder;

    private Grid<PathNode> currentGrid;
    public Grid<PathNode> ActiveGrid { get { return currentGrid; } }
    private float _gameSpeed = 1.0f;
    public float GameSpeed { get { return _gameSpeed; } }

    public delegate void UnPaused();
    public static event UnPaused onUnPaused;
    private void Start()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        return;
    }

    private void Update()
    {
        Time.timeScale = GameSpeed;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChange;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    public void OpenMainMenu()
    {
        DisablePauseMenu();
        SceneManager.LoadScene((int)GameScene.MainMenu);
    }

    public void EnablePauseMenu()
    {
        FreezeTimeScale();
        PauseMenu.SetActive(true);
    }

    public void DisablePauseMenu()
    {
        ResetTimeScale();
        PauseMenu.SetActive(false);
        onUnPaused();
    }

    public bool isPauseMenuEnabled()
    {
        return PauseMenu.activeSelf;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0) { return;}
        SetGrid();
    }

    private void SetGrid()
    {
        GridMap[] gridMaps = FindObjectsByType<GridMap>(FindObjectsSortMode.None);
        if (gridMaps.Length == 0)
        {
            Debug.Assert(gridMaps.Length > 0, "No gridmaps in scene");
        }
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

    public void FreezeTimeScale()
    {
        _gameSpeed = 0.0f;
    }

    public void ResetTimeScale()
    {
        _gameSpeed = 1.0f;
    }

    public void SetSlowMoTimeScale(float timeScale = 0.1f)
    {
        _gameSpeed = timeScale;
    }
}
