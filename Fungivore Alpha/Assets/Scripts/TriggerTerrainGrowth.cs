using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTerrainGrowth : MonoBehaviour
{

    void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.CompareTag("Trigger Spawner"))
        {
            Vector3 position = target.transform.position;
            Quaternion rotation = target.transform.rotation;

            GameObject prefab = target.GetComponent<TriggerSpawnPrefab>().spawnPrefab;
            Destroy(target.gameObject);

            Instantiate(prefab, position, rotation);
        }
    }
}
