using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDoorActivator : MonoBehaviour
{
    [SerializeField]
    private GameObject door = null;
    [SerializeField]
    private GameObject roomInterior = null;

    public LayerMask layerMask;

    private Vector3 halfExtents = new Vector3(1.5f, 1.5f, 1.5f);


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            OpenDoor();
            roomInterior.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            CloseDoor();

            if (PlayerIsNotInRoom())
            {
                roomInterior.SetActive(false);
            }
        }
    }


    private void CloseDoor()
    {
        door.SetActive(true);
        AudioManager.Instance.Play("PlayerShoot");
    }


    private void OpenDoor()
    {
        door.SetActive(false);
        AudioManager.Instance.Play("PlayerShoot");
    }


    private bool PlayerIsNotInRoom()
    {
        var pos = transform.position + transform.forward * 2f;

        Collider[] hitColliders = Physics.OverlapBox(pos, halfExtents, Quaternion.identity, layerMask);

        if (hitColliders.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
