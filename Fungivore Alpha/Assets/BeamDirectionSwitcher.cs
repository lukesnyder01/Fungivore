using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDirectionSwitcher : MonoBehaviour
{
    public ConveyorBeam beam;

    void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            beam.beamDirection = transform.forward;
        }
    }


}
