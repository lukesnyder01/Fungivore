using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
    }

    public void StartGame()
    {
        //start a new game
        Debug.Log("Starting new game");
        playerInput.EnableInput();

        FindObjectOfType<GameManager>().Invoke("Ritual", 1f);

        HideMainMenu();

    }

    public void OpenSettings()
    {
        //open the settings menu
        Debug.Log("Opening settings menu");

    }


    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


    public void HideMainMenu()
    {
        gameObject.SetActive(false);
    }

}
