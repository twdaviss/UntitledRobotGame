using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameScene startScene;
    
    public void LoadStartScene()
    {
        SceneManager.LoadScene((int)startScene);
    }
}
