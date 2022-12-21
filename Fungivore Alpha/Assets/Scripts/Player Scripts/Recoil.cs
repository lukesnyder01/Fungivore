using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public Transform recoilTarget;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float snappiness = 6;
    [SerializeField] private float returnSpeed = 5;

    private Vector3 currentPosition;
    private Vector3 targetPosition;

    private Vector3 originalPosition;

    void Awake()
    {
        originalPosition = recoilTarget.position;
    }


    void Update()
    {
        if (!PauseMenu.gameIsPaused)
        {
            targetPosition = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.fixedDeltaTime);

            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

            recoilTarget.localPosition = currentPosition;

            recoilTarget.localRotation = Quaternion.Euler(currentRotation);

        }

    }


    public void RecoilFire(float vertical, float horizontal)
    {
        targetRotation += new Vector3(vertical, Random.Range(-horizontal, horizontal), 0f);
    }


    public void RecoilJump(float x)
    {
        x = Mathf.Clamp(x, -45f, 45f);
        targetRotation += new Vector3(x, 0f, 0f);
    }


    public void RecoilStrafe(float y)
    {
        targetRotation.y = y;
    }

    public void Translate(Vector3 dir)
    {
        targetPosition = transform.position += dir;
    }

}
