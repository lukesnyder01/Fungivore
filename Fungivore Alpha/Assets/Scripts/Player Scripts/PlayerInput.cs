using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool inputEnabledByDefault = true;

    public float xInput { get; private set; }
    public float zInput { get; private set; }
    public bool shootInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool sprintInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool interactInput { get; private set; }
    public Vector2 mouseRawInput { get; private set; }
    public bool pauseButton { get; private set; }
    public bool inputEnabled { get; set; }


    void Awake()
    {
        if (inputEnabledByDefault)
        {
            EnableInput();
        }
        else
        {
            DisableInput();
        }

    }

    void Update()
    {
        pauseButton = Input.GetKeyDown(KeyCode.Escape);

        if (inputEnabled)
        {
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");
            shootInput = Input.GetMouseButton(0);
            jumpInput = Input.GetButton("Jump");
            dashInput = Input.GetKey(KeyCode.LeftControl);
            sprintInput = Input.GetKey(KeyCode.LeftShift);
            interactInput = Input.GetKeyDown(KeyCode.E);
            mouseRawInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }
        else 
        {
            xInput = 0f;
            zInput = 0f;
            shootInput = false;
            jumpInput = false;
            sprintInput = false;
            dashInput = false;
            interactInput = false;
            mouseRawInput = Vector2.zero;
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableInput()
    {
        inputEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}