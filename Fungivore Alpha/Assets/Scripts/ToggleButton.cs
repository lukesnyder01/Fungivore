using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : InteractableObject
{
    public GameObject targetObject;

    private bool toggleIsOn = true;

    public string toggleOnText = "[E] Open";
    public string toggleOffText = "[E] Close";

    void Awake()
    {
        promptText = toggleOnText;
    }


    public override void Interact()
    {
        if (toggleIsOn)
        {
            toggleIsOn = false;
            targetObject.SetActive(false);
            promptText = toggleOffText;
        }
        else
        {
            toggleIsOn = true;
            targetObject.SetActive(true);
            promptText = toggleOnText;
        }
    }



}
