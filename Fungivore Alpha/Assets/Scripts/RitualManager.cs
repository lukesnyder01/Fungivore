using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualManager : MonoBehaviour
{
    public float xShift = 0f;
    public float yShift = 0f;
    public float zShift = 0f;

    public float shiftMultiplier = 2f;
    public Vector3 shiftDelta = new Vector3(0f, 0f, 0f);

    public GameObject[] shiftTargets;
    public GameObject player;

    public GameObject door;

    private float doorOpenDistance = 25f;
    private float doorOpenSpeed = 3.3f;

    private bool doorStartedOpening = false;
    private bool doorFinishedOpening = false;


    void Update()
    {
        if (doorOpenDistance <= 0f)
        {
            doorFinishedOpening = true;
            Destroy(door);
            
            DisableRitualManager();
        }

        if (doorStartedOpening && !doorFinishedOpening)
        {
            Vector3 doorPos = door.transform.position;
            doorOpenDistance -= Time.deltaTime * doorOpenSpeed;
            door.transform.position = new Vector3(doorPos.x, doorPos.y + Time.deltaTime * doorOpenSpeed, doorPos.z);
        }
    }


    public void SetZShift(float z)
    {
        zShift = z * shiftMultiplier;
        DoWorldShift();
        zShift = 0f;
    }


    public void SetYShift(float y)
    {
        yShift = y;
        DoWorldShift();
        yShift = 0f;
    }


    public void SetXShift(float x)
    {
        xShift = x * shiftMultiplier;
        DoWorldShift();

        ActivateShiftTargets();
        Invoke("OpenDoor", 4f);
        FindObjectOfType<GameManager>().Fate();
        xShift = 0f;
    }


    void OpenDoor()
    {
        doorStartedOpening = true;
        FindObjectOfType<AudioManager>().Play("DoorGrind");
    }


    void ActivateShiftTargets()
    {
        foreach (GameObject target in shiftTargets)
        {
            target.SetActive(true);
        }
    }


    void DoWorldShift()
    {
        shiftDelta = new Vector3(xShift, yShift, zShift);

        player.GetComponent<CharacterController>().enabled = false;
        //player.transform.position += shiftDelta;

        

        foreach (GameObject target in shiftTargets)
        {
            target.transform.position += shiftDelta;
        }

        player.GetComponent<CharacterController>().enabled = true;
    }


    void DisableRitualManager()
    {
        gameObject.SetActive(false);
    }

}
