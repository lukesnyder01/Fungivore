using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogColorController : MonoBehaviour
{
    private Material[] initialMaterials;

    public Color defaultColor;

    public Material[] fogMaterials;
    public Color currentColor;
    public Color targetFogColor;
    public Color targetParticleColor;

    public ParticleSystem ps;


    private GameObject player;
    public Camera cam;



    float heightScale = 500f;
    float distanceScale = 1000f;

    float minValue = 0.01f;
    float maxValue = 0.99f;

    float maxSaturation = 0.6f;


    float baseValue = 0.4f;

    float m_playerX;
    float m_playerY;
    float m_playerZ;



    void Awake()
    {
        player = GameObject.Find("Player");
        cam.clearFlags = CameraClearFlags.SolidColor;
    }


    void Update()
    {
        var playerPos = player.transform.position;

        m_playerX = playerPos.x;
        m_playerY = playerPos.y;
        m_playerZ = playerPos.z;

        SetTargetFogColor();
        UpdateFogColor();
        UpdateParticleColor();
    }

    /*
    void CacheInitialMaterials()
    {
            initialMaterials = new Material[fogMaterials.Length];

            for (int i = 0; i < fogMaterials.Length; i++)
            {
                initialMaterials[i] = fogMaterials[i];
            }
    }
    */


    void SetTargetFogColor()
    {

        Vector3 directionToPlayer = Vector3.Normalize(new Vector3(m_playerX, 0f, m_playerZ));
        float playerAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg + 180;

        playerAngle = playerAngle / 360;


        float playerDistance = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(m_playerX, 0, m_playerZ));


        float m_Hue = playerAngle;

        float m_Saturation = Mathf.Clamp(playerDistance / distanceScale, 0f, maxSaturation);

        float m_Value = Mathf.Clamp(baseValue + (m_playerY / heightScale), minValue, maxValue);

        targetFogColor = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);

        //targetFogColor = Color.Lerp(currentColor, Color.HSVToRGB(m_Hue, m_Saturation, m_Value), Time.deltaTime * 10f);

        currentColor = targetFogColor;

    }


    void UpdateFogColor()
    {
        cam.backgroundColor = targetFogColor;

        foreach (Material m in fogMaterials)
        {
            m.SetColor("Color_8B4C3782", targetFogColor);
        }
    }


    void UpdateParticleColor()
    {
        /*
        float currentHeight = (transform.position.y + 30) / -100;
        currentHeight = currentHeight - 0.2f;
        targetParticleColor = new Color(currentHeight, currentHeight, currentHeight, 1.0f);
        */

        targetParticleColor = (Color.white - targetFogColor) * 0.5f;

        //Debug.Log(targetParticleColor);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(targetParticleColor, 0.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(0.3f, 0.25f),
                new GradientAlphaKey(0.0f, 0.4f),
                new GradientAlphaKey(0.6f, 0.75f),
                new GradientAlphaKey(0f, 1.0f),
            }
        );

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

    }


    void OnDestroy()
    {
        foreach (Material m in fogMaterials)
        {
            m.SetColor("Color_8B4C3782", defaultColor);
        }
    }

}

