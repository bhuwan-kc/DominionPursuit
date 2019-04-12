using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject creditPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        //soundManagerScript.PlaySound("buttonClick");
        SceneManager.LoadScene(1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        //soundManagerScript.PlaySound("buttonClick");

        //if pause menu not active, display it
        if (!ObjectHandler.Instance.pauseMenu.activeInHierarchy)
        {
            //pause the game 
            Time.timeScale = 0f;
            ObjectHandler.Instance.pauseMenu.SetActive(true);
        }
        else
        {
            //else pauseMenu is being displayed so hide it instead and resume the game
            Time.timeScale = 1f;
            ObjectHandler.Instance.pauseMenu.SetActive(false);
        }
    }

    public void EndGame()
    {
        Time.timeScale = 1f;
        //soundManagerScript.PlaySound("buttonClick");
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        //soundManagerScript.PlaySound("buttonClick");
        Application.Quit();
    }

    public void Setting()
    {
        //display settings
    }

    public void Credits()
    {
        //display credits
        if(creditPanel != null)
        {
            if (creditPanel.activeInHierarchy)
                creditPanel.SetActive(false);
            else
                creditPanel.SetActive(true);
        }
    }
}
