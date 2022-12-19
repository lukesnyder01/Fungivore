using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAfterTime : MonoBehaviour
{

    public float lifeSpan = 10f;
    private float currentLifeSpan;

    void Start()
    {
        currentLifeSpan = lifeSpan;
    }


    void Update()
    {
        currentLifeSpan -= Time.deltaTime;

        if (currentLifeSpan <= 0)
        {
            currentLifeSpan = lifeSpan;
            transform.parent = null;
            this.gameObject.SetActive(false);
        }
    }
}
