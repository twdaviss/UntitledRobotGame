using System.Collections;
using UnityEngine;
using RobotGame;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public enum GameScene
{
    MainMenu,
    TestScene,
};
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image meleeCooldownIcon;
    [SerializeField] private Image grappleCooldownIcon;
    [SerializeField] private TextMeshProUGUI ammoCount;

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

        DisablePauseMenu();
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

    public void UpdateHealthBar(float healthRatio)
    {
        healthBar.fillAmount = healthRatio;
    }

    public void OpenMainMenu()
    {
        StartCoroutine(ResetTimeScale());
        PauseMenu.SetActive(false);
        SceneManager.LoadScene((int)GameScene.MainMenu);
    }

    public void EnablePauseMenu()
    {
        FreezeTimeScale();
        PauseMenu.SetActive(true);
    }

    public void DisablePauseMenu()
    {
        StartCoroutine(ResetTimeScale());
        PauseMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        onUnPaused();
    }

    public bool IsPauseMenuEnabled()
    {
        return PauseMenu.activeSelf;
    }

    public bool IsOptionsMenuEnabled()
    {
        return OptionsMenu.activeSelf;
    }

    public void EnableOptionsMenu()
    {
        OptionsMenu.SetActive(true);
    }
    public void DisableOptionsMenu()
    {
        OptionsMenu.SetActive(false);
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

    public void FreezeTimeScale(float duration = 0.0f)
    {
        if(_gameSpeed != 1.0f) { return; }
        _gameSpeed = 0.0f;
        Debug.Log("Time Frozen");
        StartCoroutine(ResetTimeScale(duration));
    }

    public IEnumerator ResetTimeScale(float duration = 0.0f)
    {
        if (duration > 0.0f)
        {
            yield return new WaitForSecondsRealtime(duration);
        }
        _gameSpeed = 1.0f;
        Debug.Log("Time Reset");
        yield break;
    }

    public void SetSlowMoTimeScale(float timeScale = 0.3f)
    {
        _gameSpeed = timeScale;
    }

    public void SetMeleeCooldownUI(float meleeTime)
    {
        meleeCooldownIcon.fillAmount = 1 - meleeTime;
    }

    public void SetGrappleCooldownUI(float grappleTime)
    {
        grappleCooldownIcon.fillAmount = 1 - grappleTime;
    }

    public void SetAmmoCountUI(int count, int max)
    {
        ammoCount.text = count + " / " + max;
    }
}
