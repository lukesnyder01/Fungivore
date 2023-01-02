using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject[] spawnPrefab;

    [Range(0, 100)]
    public float spawnChance;

    public bool randomizeRotation = false;

    public bool makePrefabChild = false;

    [HideInInspector]
    public Vector3[] directionArray = new[] {
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 0f, -1f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, 0f, 1f)
    };


    void Start()
    {
        var coroutine = SpawnObject();
        StartCoroutine(coroutine);
    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(Random.Range(0.001f, 0.01f));

        Random.InitState(transform.position.GetHashCode() % 100000);

        if (Random.Range(0, 100) < spawnChance)
        {
            var selectedPrefab = spawnPrefab[(Random.Range(0, spawnPrefab.Length))];
            GameObject newSpawnedObject = Instantiate(selectedPrefab, transform.position, transform.rotation);

            if (randomizeRotation)
            {
                newSpawnedObject.transform.forward = directionArray[Random.Range(0, 4)];
            }

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
