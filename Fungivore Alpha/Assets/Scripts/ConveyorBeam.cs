using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeam : MonoBehaviour
{
    public float beamSpeed = 10;
    public Vector3 beamDirection;

    public GameObject beamRingParticles;
    public Vector3 ringOrientation;



    void Awake()
    {
        beamDirection = transform.forward;

        ringOrientation = new Vector3(Mathf.Abs(beamDirection.x), 0, Mathf.Abs(beamDirection.z));


    }


}
