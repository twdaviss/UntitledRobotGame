using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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
