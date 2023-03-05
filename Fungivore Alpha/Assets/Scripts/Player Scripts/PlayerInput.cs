using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float xInput { get; private set; }
    public float zInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool sprintInput { get; private set; }
    public bool interactInput { get; private set; }
    public Vector2 mouseRawInput { get; private set; }

    public bool playerCanMove { get; set; }
    public bool playerCanLook { get; set; }

    void Awake()
    {
        playerCanMove = false;
        playerCanLook = false;
    }


    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");
        sprintInput = Input.GetKey(KeyCode.LeftShift);
        interactInput = Input.GetKeyDown(KeyCode.E);
        mouseRawInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));


        if (Input.GetKeyDown(KeyCode.P))
        {
            playerCanMove = !playerCanMove;
            playerCanLook = !playerCanLook;
        }


    }
}