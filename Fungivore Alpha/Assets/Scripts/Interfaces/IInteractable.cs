using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string PromptText { get; set; }

    void StartFocus();

    void LoseFocus();

    void Interact();
}
