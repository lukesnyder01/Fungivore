using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticlePosition : MonoBehaviour
{
    public float raycastDist = 15f;

    public Transform gunTransform;

    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("PlayerBullet");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        transform.forward = gunTransform.forward;

        if (Physics.Raycast(gunTransform.position, gunTransform.forward, out hit, raycastDist, ~layerMask))
        {


            transform.position = hit.point;
        }

    }
}
