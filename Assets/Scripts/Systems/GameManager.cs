using System.Collections;
using UnityEngine;
using RobotGame;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public enum GameScene
{
    Mainmenu,
    Junkyard,
    Parkinglot,
    Building,
    Street,
    Alley,
};
public class GameManager : MonoBehaviour
{
    [SerializeField] GameScene startScene;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private GameObject TutorialPanel;
    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private GameObject GameplayUI;
    [SerializeField] private TextMeshProUGUI TutorialText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image meleeCooldownIcon;
    [SerializeField] private Image grappleCooldownIcon;
    [SerializeField] private Image scrapCooldownIcon;
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
        if (Instance == null)
        {
            //First run, set the instance
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        DisableMenus();
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

    #region Initialize

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
    #endregion

    #region UI
    public void EnablePauseMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu") { return; }
        FreezeTimeScale();
        PauseMenu.SetActive(true);
    }

    public void DisableMenus()
    {
        StartCoroutine(ResetTimeScale());
        PauseMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        TutorialPanel.SetActive(false);
        DeathScreen.SetActive(false);
        onUnPaused();
    }
    public void EnableDeathScreen()
    {
        FreezeTimeScale();
        InputManager.playerControls.Gameplay.Disable();
        DeathScreen.SetActive(true);
    }

    public void EnableTutorial()
    {
        string scrap = InputManager.GetBindingName("Scrap", 0) + " or " + InputManager.GetBindingName("Scrap", 1);
        string magnetize = InputManager.GetBindingName("Magnetize", 0) + " or " + InputManager.GetBindingName("Magnetize", 1);
        string spin = InputManager.GetBindingName("Spin", 0) + " or " + InputManager.GetBindingName("Spin", 1);
        string grapple = InputManager.GetBindingName("Grapple", 0) + " or " + InputManager.GetBindingName("Grapple", 1);

        TutorialText.text = 
            "<color=\"blue\">Scrap:</color> Press [" + scrap + "] to Shoot Scrap at enemies to deal minor <color=\"red\">damage</color> and major <color=\"red\">stun</color>\r\n" +
            "<color=\"blue\">Magnetize:</color>  Press [" + magnetize +  "] to Magnetize your shot scrap to return them to you and <color=\"red\">hit enemies on the way back</color>\r\n" +
            "<color=\"blue\">Spin:</color>  Press [" + spin + "] to Spin around to damage and <color=\"red\">knock enemies back.</color> Knocked back enemies can hit other enemies to <color=\"red\">knock them back as well.</color>\r\n" +
            "<color=\"blue\">Grapple:</color>  Hold [" + grapple + "] to Charge up and aim at certain objects or enemies and release to <color=\"red\">fly towards them.</color> Spin attack an enemy while grappling to <color=\"red\">deal extra damage.</color>\r\n" +
            "<color=\"blue\">Stun:</color> When enemies stun bars fill completely they will enter a <color=\"red\">weakened state.</color> Damage stunned enemies for <color=\"red\">large boost in damage.</color>\r\n";
        TutorialPanel.SetActive(true);
    }
    public void DisableTutorial()
    {
        TutorialPanel.SetActive(false);
    }
    public bool IsTutorialEnabled()
    {
        return TutorialPanel.activeSelf;
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

    public void UpdateHealthBar(float healthRatio)
    {
        healthBar.fillAmount = healthRatio;
    }

    public void SetMeleeCooldownUI(float meleeTime)
    {
        meleeCooldownIcon.fillAmount = 1 - meleeTime;
    }

    public void SetGrappleCooldownUI(float grappleTime)
    {
        grappleCooldownIcon.fillAmount = 1 - grappleTime;
    }

    public void SetScrapCooldownUI(float magnetizeTime)
    {
        scrapCooldownIcon.fillAmount = 1 - magnetizeTime;
    }

    public void SetAmmoCountUI(int count, int max)
    {
        ammoCount.text = count + " / " + max;
    }
    #endregion

    #region TimeScale
    public void FreezeTimeScale(float duration = 0.0f)
    {
        if(_gameSpeed != 1.0f) { return; }
        _gameSpeed = 0.0f;
        Debug.Log("Time Frozen");
        if (duration > 0)
        {
            StartCoroutine(ResetTimeScale(duration));
        }
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
    #endregion

    #region SceneManagement

    public void OpenMainMenu()
    {
        StartCoroutine(ResetTimeScale());
        InputManager.playerControls.Menu.Enable();
        InputManager.playerControls.Gameplay.Disable();
        PauseMenu.SetActive(false);
        SceneManager.LoadScene("Mainmenu");
    }

    public void LoadStartScene()
    {
        InputManager.playerControls.Menu.Disable();
        InputManager.playerControls.Gameplay.Enable();
        SceneManager.LoadScene(startScene.ToString().Prettify());
        GameplayUI.SetActive(true);
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0) { return;}
        SetGrid();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
