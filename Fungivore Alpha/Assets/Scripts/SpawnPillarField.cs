using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CombineMesh))]
public class SpawnPillarField : MonoBehaviour
{
    CombineMesh combineMesh;

    public GameObject pillar;
    public GameObject pillarFieldMeshes;

    public float waitTime = 0.01f;

    public int minHeight = 2;
    public int maxHeight = 100;


    public float pillarSpawnChance;

    private Vector3 pos;

    private int xSize;
    private int zSize;

    private int xOffset;
    private int zOffset;



    public int spawnsPerWait = 10;
    private int spawnsSinceLastWait = 0;

    private Vector3 overlapBoxPad = new Vector3(0.05f, 0.05f, 0.05f);



    // Start is called before the first frame update
    void Start()
    {
        combineMesh = GetComponent<CombineMesh>();

        xSize = Mathf.RoundToInt(transform.localScale.x - 2);
        zSize = Mathf.RoundToInt(transform.localScale.z - 2);

        pos = transform.position;

        //start at upper left hand corner -1/2xSize, -1/2zSize
        xOffset = Mathf.RoundToInt(pos.x - ((xSize - 1) / 2));
        zOffset = Mathf.RoundToInt(pos.z - ((zSize - 1) / 2));

        StartCoroutine(SpawnPillars());
    }


    public IEnumerator SpawnPillars()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                Vector3 spawnPos = new Vector3(xOffset + i, pos.y, zOffset + j);

                //Random.InitState(spawnPos.GetHashCode() % 100000);

                //generates a random odd pillar height
                int pillarHeight = Random.Range(minHeight, maxHeight) * 2 + 1;

                spawnPos.y = 1 + spawnPos.y + Mathf.RoundToInt(pillarHeight / 2);

                Random.InitState(spawnPos.GetHashCode() % 100000);

                if (Random.Range(0f, 100f) < pillarSpawnChance)
                {
                    int pillarWidth = Random.Range(0, 2) * 2 + 1;

                    Vector3 pillarScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);

                    //check to see if there's anything where the pillar would spawn
                    Collider[] hitColliders = Physics.OverlapBox(spawnPos, pillarScale / 2f - overlapBoxPad, Quaternion.identity);

                    if (hitColliders.Length == 0)
                    {
                        GameObject newPillar = Instantiate(pillar, spawnPos, Quaternion.identity);

                        newPillar.transform.localScale = pillarScale;

                        newPillar.transform.parent = pillarFieldMeshes.transform;

                        spawnsSinceLastWait++;

                        if (spawnsSinceLastWait >= spawnsPerWait)
                        {
                            spawnsSinceLastWait = 0;
                            yield return new WaitForSeconds(waitTime);
                        }
                    }
                    
                }

            }
        }

        combineMesh.Combine(pillarFieldMeshes);

    }

}
