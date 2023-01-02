using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenDamageIndicator : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public Material screenDamageMaterial;

    private float bloodFadeSpeed = 0.005f;
    private float maximumOpacity = 0.5f;

    private Vignette vignette;
    private Color targetColor;
    private float targetOpacity;
    private Color startColor;


    void Awake()
    {
        startColor = screenDamageMaterial.color;
        startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);



        postProcessVolume = FindObjectOfType<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out vignette);
    }


    public void MakeScreenBloody(float bloodAmount)
    {
        targetOpacity += bloodAmount;
    }


    void Update()
    {
        float damagePercent = 1 - PlayerStats.healthPercent;

        float shiftedDamagePercent = Mathf.Clamp(damagePercent - 0.3f, 0, 1);

        if (targetOpacity > maximumOpacity)
        {
            targetOpacity = maximumOpacity;
        }

        if (targetOpacity > shiftedDamagePercent && targetOpacity > 0)
        {
            targetOpacity -= bloodFadeSpeed;
        }
        else if (targetOpacity < shiftedDamagePercent && targetOpacity < maximumOpacity)
        {
            targetOpacity += bloodFadeSpeed;
        }

        screenDamageMaterial.color = new Color(startColor.r, startColor.g, startColor.b, targetOpacity);
    }


    void OnDestroy()
    {
        screenDamageMaterial.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

}
