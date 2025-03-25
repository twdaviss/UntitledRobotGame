using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameScene startScene;
    [SerializeField] GameObject tutorialPanel;
    
    public void LoadStartScene()
    {
        SceneManager.LoadScene((int)startScene);
    }

    public void EnableTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void DisableTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
