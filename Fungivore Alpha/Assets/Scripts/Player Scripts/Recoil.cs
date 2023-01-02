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

    private Transform playerCam;

    void Awake()
    {
        playerCam = Camera.main.transform;
        recoilTarget.position = playerCam.position;
    }


    void Update()
    {
        if (!PauseMenu.gameIsPaused)
        {
            //UpdateRotation();
            UpdateTranslation();
        }
    }


    void UpdateRotation()
    {

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        recoilTarget.localRotation = Quaternion.Euler(currentRotation);
    }



    void UpdateTranslation()
    {
        targetPosition = Vector3.Lerp(targetPosition, playerCam.position, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.fixedDeltaTime);

        var difference = playerCam.position - targetPosition;
        var direction = difference.normalized;
        var distance = Mathf.Min(0.01f, difference.magnitude);


        recoilTarget.position = currentPosition + direction * distance;
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

    public void TranslationRecoil()
    {
        Debug.Log(recoilTarget.forward);

        targetPosition = playerCam.position + (recoilTarget.forward * -0.01f);


        //Debug.Log(targetPosition);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPosition, 0.1f);
    }

}
