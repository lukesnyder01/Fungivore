using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnFromList : MonoBehaviour
{
    public GameObject[] spawnPrefab;

    public bool makePrefabChild = false;

    [Range(0, 100)]
    public float spawnChance;

    void Start()
    {
        Random.InitState(transform.position.GetHashCode() % 100000);

        if (Random.Range(0, 100) < spawnChance)
        {
            Random.InitState(transform.position.GetHashCode() % 100000);
            var selectedPrefab = spawnPrefab[(Random.Range(0, spawnPrefab.Length))];


            GameObject newSpawnedObject = Instantiate(selectedPrefab, transform.position, transform.rotation);

            if (!makePrefabChild)
            {
                transform.parent = null;
                newSpawnedObject.transform.parent = null;
            }
            else
            {
                newSpawnedObject.transform.parent = transform.parent;
                transform.parent = null;
            }

        }

        Destroy(gameObject);
    }

}
