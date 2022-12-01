using UnityEngine;
using System.Collections;

public class ParticleSystemColor : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        float currentHeight = (transform.position.y + 30) / -100;
        currentHeight = currentHeight - 0.2f;

        Color targetColor = new Color(currentHeight, currentHeight, currentHeight, 1.0f);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(targetColor, 0.0f) },
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

}