using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRateDisplay : MonoBehaviour
{
    private Text displayText;

    private int count;
    private float totalTime;
    public int samples = 100;


    public void Start()
    {
        displayText = gameObject.GetComponent<Text>();
        count = samples;
        totalTime = 0f;
    }


    void Update()
    {
        count -= 1;
        totalTime += Time.deltaTime;

        if (count <= 0)
        {
            float fps = samples / totalTime;
            //displayText.text = fps.ToString() + "FPS"; // your way of displaying number. Log it, put it to text object…
            fps = Mathf.Round(fps * 10f) / 10f;
            string fpsToDisplay = fps.ToString("0.0");
            displayText.text = ("FPS: " + fpsToDisplay);
            totalTime = 0f;
            count = samples;
        }
    }
}
