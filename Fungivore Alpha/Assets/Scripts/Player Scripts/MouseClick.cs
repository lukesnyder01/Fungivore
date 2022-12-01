using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public GameObject branch;
    public GameObject rib;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2f) && hit.transform.tag == "Spawner Rib")
            {
                Instantiate(rib, hit.transform.position, Quaternion.identity);
                Destroy(hit.transform.gameObject);
            }

                if (PlayerStats.sporesInventory >= 1) 
            {
                //MakeBlock();
            }
            
        }
    }

    void MakeBlock()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 1.0f))
        {
            if (Physics.Raycast(ray, out hit, 5.0f))
            {
                Vector3 blockPos = hit.point + hit.normal / 2.0f;
                blockPos.x = (float)System.Math.Round(blockPos.x, MidpointRounding.AwayFromZero);
                blockPos.y = (float)System.Math.Round(blockPos.y, MidpointRounding.AwayFromZero);
                blockPos.z = (float)System.Math.Round(blockPos.z, MidpointRounding.AwayFromZero);
                Instantiate(branch, blockPos, Quaternion.identity);
                PlayerStats.sporesInventory--;
            }
        }
    }

}
