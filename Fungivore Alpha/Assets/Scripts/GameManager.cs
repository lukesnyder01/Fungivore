using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int seed;

    public int targetFrameRate = 240;
    public float cameraRenderDistance = 70f;

    public GameObject centerScreenText;
    public Transform centerScreenTextLocation;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        RandomUtility.InitializeGlobalSeed(seed);
        Debug.Log(seed);



        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        QualitySettings.vSyncCount = 0;

        SetTargetFramerate(targetFrameRate);

        SetCameraRenderDistance(cameraRenderDistance);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            seed++;
            RandomUtility.InitializeGlobalSeed(seed);
            Debug.Log(seed);
        }

    }


    public void Ritual()
    {
        FindObjectOfType<AudioManager>().Play("theme00");

        var player = GameObject.Find("Player").GetComponent<PlayerUI>().centerScreenTextLocation;

        GameObject ritualText = Instantiate(centerScreenText, centerScreenTextLocation);
        ritualText.GetComponent<TextMeshPro>().text = "Ritual";

        SceneManager.LoadSceneAsync(1);

    }

    public void Fate()
    {
        //FindObjectOfType<AudioManager>().FadeOut("theme00", 2);

        //FindObjectOfType<AudioManager>().Play("theme02");
        GameObject ritualText = Instantiate(centerScreenText, centerScreenTextLocation);
        ritualText.GetComponent<TextMeshPro>().text = "Fate";
    }

    public void SetCameraRenderDistance(float distance)
    {
        Camera.main.farClipPlane = distance;
    }

    public void SetTargetFramerate(int framerate)
    {
        Application.targetFrameRate = framerate;
    }




}
