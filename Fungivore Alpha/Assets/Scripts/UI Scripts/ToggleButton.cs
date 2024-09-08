using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour, IInteractable
{
    public GameObject targetObject;

    private bool toggleIsOn = true;

    public string toggleOnText = "[E] Open";
    public string toggleOffText = "[E] Close";

    void Awake()
    {
        PromptText = toggleOnText;
    }

    public string PromptText { get; set; } = "E";

    public void StartFocus()
    {

    }

    public void LoseFocus()
    {

    }

    public void Interact()
    {
        if (toggleIsOn)
        {
            toggleIsOn = false;
            targetObject.SetActive(false);
            PromptText = toggleOffText;
        }
        else
        {
            toggleIsOn = true;
            targetObject.SetActive(true);
            PromptText = toggleOnText;
        }
    }





}
