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

    public float chanceOfDoubleHeightPillar;

    public float pillarSpawnChance;

    private Vector3 pos;

    private int xSize;
    private int zSize;

    private int xOffset;
    private int zOffset;

    public int spawnsPerWait = 10;
    private int spawnsSinceLastWait = 0;



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

        Random.InitState(transform.position.GetHashCode() % 100000);

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

                if (Random.Range(0f, 100f) < pillarSpawnChance)
                {
                    //generates a pillar height
                    float pillarHeight = Random.Range(minHeight, maxHeight);

                    pillarHeight = Mathf.RoundToInt(pillarHeight);

                    //chance to double a pillar's height
                    if (Random.Range(0f, 100f) < chanceOfDoubleHeightPillar)
                    {
                        pillarHeight *= 2f;
                    }

                    int pillarWidth = Random.Range(0, 5) * 2 + 1;


                    RaycastHit hit;
                    Vector3 halfExtents = new Vector3(pillarWidth / 2, 0.1f, pillarWidth / 2);

                    //see if a pillar of the selected height would hit something
                    if (Physics.BoxCast(spawnPos, halfExtents, Vector3.up, out hit, Quaternion.identity, pillarHeight))
                    {
                        pillarHeight = Mathf.RoundToInt(hit.point.y - pos.y);
                    }

                    spawnPos.y = (pos.y + 1) + ((pillarHeight - 1) / 2);

                    Vector3 pillarScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);

                    Transform newPillar = Instantiate(pillar, spawnPos, Quaternion.identity).transform;

                    newPillar.localScale = pillarScale;
                    newPillar.parent = pillarFieldMeshes.transform;


                    spawnsSinceLastWait++;

                    if (spawnsSinceLastWait >= spawnsPerWait)
                    {
                        spawnsSinceLastWait = 0;
                        yield return new WaitForSeconds(waitTime);
                    }
                }
            }
        }

        combineMesh.Combine(pillarFieldMeshes);

    }

}
