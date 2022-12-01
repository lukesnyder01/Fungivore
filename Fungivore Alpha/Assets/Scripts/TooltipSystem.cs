using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;

    public Tooltip tooltip;


    public void Awake()
    {
        current = this;
        current.tooltip.gameObject.SetActive(false);
    }


    public static void ResetText(string content, string header = "")
    {
        current.tooltip.SetText(content, header);
    }


    public static void Show(Vector2 position, string content, string header = "")
    {
        current.tooltip.transform.position = position;

        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
    }


    public static void Hide()
    {

        current.tooltip.gameObject.SetActive(false);

    }

}
