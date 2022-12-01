using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 2;
    [SerializeField] private float swayMultiplier = 2;

    float mouseX;
    float mouseY;
    Quaternion rotationX;
    Quaternion rotationY;
    Quaternion targetRotation;


    private void Update()
    {
        if (!PauseMenu.gameIsPaused)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
            mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

            rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
            rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

            targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.fixedDeltaTime);
        }
    }
}
