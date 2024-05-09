using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeam : MonoBehaviour
{
    public GameObject beamRingParticles;
    public Vector3 beamOrientation;
    public float beamSpeed = 10f;

    void Awake()
    {
        beamOrientation = new Vector3
            (
                Mathf.Abs(transform.forward.x),
                Mathf.Abs(transform.forward.y), 
                Mathf.Abs(transform.forward.z)
            );
    }
}
