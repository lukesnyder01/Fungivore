using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRespawner : MonoBehaviour
{
    public GameObject spawnPrefab;

    [HideInInspector]
    public int prefabRespawnCount;

    [HideInInspector]
    public int prefabDir;


    void Start()
    {
        var spawnedPrefab = Instantiate(spawnPrefab, transform.position, transform.rotation);

        spawnedPrefab.GetComponentInChildren<MazeAnt>().antDir = prefabDir;

        spawnedPrefab.GetComponentInChildren<MazeAnt>().respawnCount = prefabRespawnCount;

        spawnedPrefab.transform.parent = this.transform.root;

        Destroy(this.gameObject);
    }


}