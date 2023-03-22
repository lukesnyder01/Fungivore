using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused;

    public GameObject pauseMenuUI;

    public Text spinesPerShotText;


    public PlayerStats playerStats;


    private bool fadeOutAudio = false;
    private float audioFadeSpeed = 10f;

    private PlayerInput playerInput;


    void Awake()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        pauseMenuUI.SetActive(false);
    }


    void Update()
    {
        if (playerInput.pauseButton)
        {
            if (gameIsPaused)
            {
                playerInput.inputEnabled = true;
                Resume();
            }
            else
            {
                playerInput.inputEnabled = false;
                Pause();
            }
        }


        if (fadeOutAudio)
        {
            if (AudioListener.volume > 0f)
            {
                AudioListener.volume -= Time.deltaTime * audioFadeSpeed;
            }
            else 
            {
                AudioListener.volume = 0f;
                AudioListener.pause = true;
                Time.timeScale = 0f;
            }
        }
        else 
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            if (AudioListener.volume < 1f)
            {
                AudioListener.volume += Time.deltaTime * audioFadeSpeed;
            }
            else
            {
                AudioListener.volume = 1f;
            }
        }
    }


    void Resume()
    {
        fadeOutAudio = false;
        TooltipSystem.Hide();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
    }


    void Pause()
    {
        fadeOutAudio = true;
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        TooltipSystem.Hide();
    }


    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


}
