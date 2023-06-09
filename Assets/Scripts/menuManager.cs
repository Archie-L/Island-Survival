using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    public GameObject mainMenu, options;
    public bool isMainScreen;
    public GameObject player;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (isMainScreen)
        {
            //SceneManager.LoadScene("newWorld");
            gameManager.instance.LoadGame();
        }
        else
        {
            player.GetComponent<PlayerMovement>().CloseEscapeMenu();
        }
    }

    public void ToOptions()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
    }
    public void ToMain()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
    }

    public void Exit()
    {
        if (isMainScreen)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene("loadingScene");
        }
    }
}
