using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string seed;
    System.Random pseudoRandom;
    public int targetFrameRate = 240;
    public float cameraRenderDistance = 70f;

    public GameObject centerScreenText;
    public Transform centerScreenTextLocation;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }



        pseudoRandom = new System.Random(seed.GetHashCode());


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        QualitySettings.vSyncCount = 0;

        SetTargetFramerate(targetFrameRate);

        SetCameraRenderDistance(cameraRenderDistance);

        Invoke("Ritual", 2f);
    }

    public void Ritual()
    {
        FindObjectOfType<AudioManager>().Play("theme00");
        GameObject ritualText = Instantiate(centerScreenText, centerScreenTextLocation);
        ritualText.GetComponent<TextMeshPro>().text = "Ritual";
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
