using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeOffset : MonoBehaviour
{
    public Material material;


    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        Random.InitState(transform.position.GetHashCode() % 100000);


        float xOffset = Random.Range(0f, 1f);

        material.SetFloat("Vector1_EDC0D4C1", xOffset);

        float yOffset = Random.Range(0f, 1f);

        material.SetFloat("Vector1_DDF87003", xOffset);

        //Vector1_EDC0D4C1
        //Vector1_DDF87003

    }

}
