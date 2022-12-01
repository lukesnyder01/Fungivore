using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public float fallAmount = 0.01f;
    public float shrinkAmount = 0.02f;
    public float stretchAmount = 0.02f;

    public AnimationCurve alphaFadeCurve;

    public float fadeDuration = 10f;

    float currentTime = 0f;

    private float despawnTimer = 0f;

    public TextMeshPro textMeshPro;

    void Start()
    {

    }


    void Update()
    {
        currentTime += Time.deltaTime;

        float targetOpacity = alphaFadeCurve.Evaluate(currentTime / fadeDuration);

        //fade vertex color alpha to make text become gradually more transparent
        var targetColor = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, targetOpacity);
        textMeshPro.color = targetColor;



        //move downward
        var targetPosition = new Vector3(transform.position.x, transform.position.y - fallAmount * Time.deltaTime, transform.position.z);
        transform.position = targetPosition;

        //stretch vertically and shrink horizontally
        var targetScale = new Vector3(transform.localScale.x - shrinkAmount * Time.deltaTime, transform.localScale.y + stretchAmount * Time.deltaTime, transform.localScale.z);
        transform.localScale = targetScale;



        //add to despawn timer
        despawnTimer += Time.deltaTime;

        //destroy if despawn timer exceeds despawn time
        if (despawnTimer > fadeDuration)
        {
            Destroy(gameObject);
        }



    }
}
