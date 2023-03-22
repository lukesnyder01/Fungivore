using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{


    public void UpdateText(float value)
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = value.ToString();
    }

}
