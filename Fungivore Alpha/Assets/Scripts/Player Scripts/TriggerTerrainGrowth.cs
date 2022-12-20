using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTerrainGrowth : MonoBehaviour
{
    void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.CompareTag("Trigger Spawner"))
        {
            var position = target.transform.position;
            var rotation = target.transform.rotation;

            var antRespawner = target.GetComponent<AntRespawner>();
            var prefabToSpawn = antRespawner.spawnPrefab;

            Destroy(target.gameObject);

            var newAnt = Instantiate(prefabToSpawn, position, rotation);

            if (newAnt.TryGetComponent(out Ant ant))
            {
                ant.respawnCount = antRespawner.respawnsLeft;
            }
        }
    }
}
