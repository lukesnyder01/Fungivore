using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class ScreenDamageIndicator : MonoBehaviour
{
    private float currentWeight;
    private float targetWeight;


    private float fadeSpeed = 0.5f;

    public Volume volume;

    private float damagePercent;



    public void MakeScreenBloody(float bloodAmount)
    {
        currentWeight += bloodAmount;
    }


    void Update()
    {
        if (PlayerStats.healthPercent > 0.5f)
        {
            targetWeight = 0f;
        }
        else
        {
            targetWeight = (0.5f - PlayerStats.healthPercent) / 0.5f;
        }

        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * fadeSpeed);

        volume.weight = currentWeight;
    }
}
