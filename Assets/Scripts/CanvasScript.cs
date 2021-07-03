using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
   public string sceneName;
    public AudioSource bgm;
    
    public void PlayGame()
    {
            SceneManager.LoadScene(sceneName);
    }
    public void levselect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void lobselect()
    {
        SceneManager.LoadScene("LobbySelect");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Application.targetFrameRate = 300;
    }
    void Update()
        {


        
        
        
            if (Input.GetKeyDown("escape") || Input.GetButtonDown("GoBack"))
            {
                SceneManager.LoadScene("MainMenu");
            }
            //else
            //{
               // SceneManager.LoadScene(sceneName);
           // }
            
        }

    
            
}