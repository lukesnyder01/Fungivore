using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CombineMesh))]
public class SpawnPillarField : MonoBehaviour
{
    CombineMesh combineMesh;

    public GameObject pillar;
    public GameObject pillarFieldMeshes;

    public float pillarSpawnChance;


    // Start is called before the first frame update
    void Start()
    {
        combineMesh = GetComponent<CombineMesh>();


        int xSize = Mathf.RoundToInt(transform.localScale.x - 7);
        int zSize = Mathf.RoundToInt(transform.localScale.z - 7);

        Vector3 pos = transform.position;

        //start at upper left hand corner -1/2xSize, -1/2zSize
        int xOffset = Mathf.RoundToInt(pos.x - ((xSize - 1) / 2));
        int zOffset = Mathf.RoundToInt(pos.z - ((zSize - 1) / 2));

       
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                if (Random.Range(0f, 100f) < pillarSpawnChance)
                {
                    Vector3 spawnPos = new Vector3(xOffset + i, pos.y + 0.5f, zOffset + j);

                    int pillarHeight = Random.Range(10, 100);
                    int pillarWidth = Random.Range(0, 2) * 2 + 1;
                    Vector3 pillarScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);

                    GameObject newPillar = Instantiate(pillar, spawnPos, Quaternion.identity);

                    newPillar.transform.localScale = pillarScale;

                    newPillar.transform.parent = pillarFieldMeshes.transform;
                }
            }
        }

        combineMesh.Combine(pillarFieldMeshes);

    }





}
