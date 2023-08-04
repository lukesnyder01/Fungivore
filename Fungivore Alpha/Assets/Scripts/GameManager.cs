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

    System.Random pseudoRandom;
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

        pseudoRandom = new System.Random(seed.GetHashCode());

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        QualitySettings.vSyncCount = 0;

        SetTargetFramerate(targetFrameRate);

        SetCameraRenderDistance(cameraRenderDistance);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            seed = UnityEngine.Random.Range(0, 10000000);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
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
