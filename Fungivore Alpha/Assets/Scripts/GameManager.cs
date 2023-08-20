using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int targetFrameRate = 240;
    public float cameraRenderDistance = 70f;

    public GameObject centerScreenText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        QualitySettings.vSyncCount = 0;

        SetTargetFramerate(targetFrameRate);

        SetCameraRenderDistance(cameraRenderDistance);



    }



    void Start()
    {
        AudioManager.Instance.Play("theme03WalkBetweenTheWaters");
    }


    public void WalkBetweenTheWaters()
    {
        SceneManager.LoadSceneAsync(0);
        AudioManager.Instance.Play("theme03WalkBetweenTheWaters");
    }


    public void Ritual()
    {
        RandomUtility.ResetGlobalSeed();
        ShowCenterScreenText("Ritual");
    }


    public void LoadMainScene()
    {
        AudioManager.Instance.FadeOut("theme03WalkBetweenTheWaters", 3);
        SceneManager.LoadSceneAsync(1);
    }



    public void Fate()
    {
        AudioManager.Instance.FadeOut("theme00", 2);
        AudioManager.Instance.Play("theme02");
        ShowCenterScreenText("Fate");
    }



    public void SetCameraRenderDistance(float distance)
    {
        Camera.main.farClipPlane = distance;
    }

    public void SetTargetFramerate(int framerate)
    {
        Application.targetFrameRate = framerate;
    }



    public void ShowCenterScreenText(string text)
    {
        var centerScreenTextLocation = GameObject.Find("Player").GetComponent<PlayerUI>().centerScreenTextLocation;

        GameObject ritualText = Instantiate(centerScreenText, centerScreenTextLocation);

        ritualText.GetComponent<TextMeshPro>().text = text;
    }


}
