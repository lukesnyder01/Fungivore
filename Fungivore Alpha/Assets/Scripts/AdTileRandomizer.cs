using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdTileRandomizer : MonoBehaviour
{
    private Material material;

    private float maxResetTime = 0.5f;
    private float minResetTime = 0.05f;

    private float scrollSpeed = 0.1f;

    private float xScrollSpeed;
    private float yScrollSpeed;


    private float currentTimer;




    void Start()
    {
        material = GetComponent<MeshRenderer>().material;

        SetTimer();
        RandomizeScrollSpeed();
        RandomizeUVOffset();



        //Random.InitState(transform.position.GetHashCode() % 100000);
    }


    void Update()
    {
        currentTimer -= Time.deltaTime;

        ScrollUVs();

        if (currentTimer < 0)
        {
            RandomizeScrollSpeed();
            RandomizeUVOffset();
            SetTimer();
        }
    }


    void SetTimer()
    {
        currentTimer = Random.Range(minResetTime, maxResetTime);
    }


    void RandomizeScrollSpeed()
    {
        xScrollSpeed = Random.Range(-scrollSpeed, scrollSpeed);
        yScrollSpeed = Random.Range(-scrollSpeed, scrollSpeed);
    }


    void ScrollUVs()
    {
        var currentXOffset = material.GetFloat("Vector1_EDC0D4C1");
        var currentYOffset = material.GetFloat("Vector1_DDF87003");

        material.SetFloat("Vector1_EDC0D4C1", currentXOffset += xScrollSpeed * Time.deltaTime);
        material.SetFloat("Vector1_DDF87003", currentYOffset += yScrollSpeed * Time.deltaTime);
    }



    void RandomizeUVOffset()
    {
        float xOffset = Random.Range(0f, 1f);
        material.SetFloat("Vector1_EDC0D4C1", xOffset);

        float yOffset = Random.Range(0f, 1f);
        material.SetFloat("Vector1_DDF87003", yOffset);
    }


}
