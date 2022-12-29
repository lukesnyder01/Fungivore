using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IToggleable
{
    public bool isOpen = false;

    public void Toggle()
    {
        isOpen = !isOpen;
        gameObject.SetActive(isOpen);
    }
}
