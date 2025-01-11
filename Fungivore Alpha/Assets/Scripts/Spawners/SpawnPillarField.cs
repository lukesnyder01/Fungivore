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

    public int minHeight = 5;
    public int maxHeight = 100;

    public float chanceOfDoubleHeightPillar;

    public float pillarSpawnChance;

    public bool pillarsSpawnDownward = true;



    private Vector3 boxCastDir;
    private Vector3 halfExtents;

    private Vector3 basePosition;

    private int xSize;
    private int zSize;

    private int xOffset;
    private int zOffset;

    public int spawnsPerWait = 10;
    private int spawnsSinceLastWait = 0;


    private Vector3 pillarSpawnPos;
    private float pillarHeight;
    private int pillarWidth;




    // Start is called before the first frame update
    void Start()
    {
        if (pillarsSpawnDownward)
        {
            boxCastDir = Vector3.down;
        }
        else
        {
            boxCastDir = Vector3.up;
        }


        combineMesh = GetComponent<CombineMesh>();

        xSize = Mathf.RoundToInt(transform.localScale.x - 2);
        zSize = Mathf.RoundToInt(transform.localScale.z - 2);

        basePosition = transform.position;

        //start at upper left hand corner -1/2xSize, -1/2zSize
        xOffset = Mathf.RoundToInt(basePosition.x - ((xSize - 1) / 2));
        zOffset = Mathf.RoundToInt(basePosition.z - ((zSize - 1) / 2));

        Random.InitState(transform.position.GetHashCode() % 100000);

        StartCoroutine(SpawnPillars());
    }


    public IEnumerator SpawnPillars()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                pillarSpawnPos = new Vector3(xOffset + i, basePosition.y, zOffset + j);

                //Random.InitState(spawnPos.GetHashCode() % 100000);

                if (Random.Range(0f, 100f) < pillarSpawnChance)
                {
                    SetPillarDimensions();

                    BoxcastToAdjustHeight();

                    if (pillarHeight > 0)
                    {
                        SetPillarYPosition();

                        SpawnPillar();
                    }

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


    void SetPillarDimensions()
    {
        //generates a pillar height
        pillarHeight = Random.Range(minHeight, maxHeight);

        pillarHeight = Mathf.RoundToInt(pillarHeight);

        //chance to double a pillar's height
        if (Random.Range(0f, 100f) < chanceOfDoubleHeightPillar)
        {
            pillarHeight *= 2f;
        }

        pillarWidth = Random.Range(0, 2) * 2 + 1;
    }


    void BoxcastToAdjustHeight()
    {
        RaycastHit hit;
        halfExtents = new Vector3(pillarWidth / 2, 0.1f, pillarWidth / 2);

        //see if a pillar of the selected height would hit something
        if (Physics.BoxCast(pillarSpawnPos, halfExtents, boxCastDir, out hit, Quaternion.identity, pillarHeight))
        {
            pillarHeight = Mathf.RoundToInt(hit.point.y - basePosition.y);
            pillarHeight = Mathf.Abs(pillarHeight);
            pillarHeight -= 1;
        }
    }


    void SetPillarYPosition()
    {
        if (pillarsSpawnDownward)
        {
            pillarSpawnPos.y = (basePosition.y - 1) - ((pillarHeight - 1) / 2);
        }
        else
        {
            pillarSpawnPos.y = (basePosition.y + 1) + ((pillarHeight - 1) / 2);
        }
    }


    void SpawnPillar()
    {
        Vector3 pillarScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);

        Transform newPillar = Instantiate(pillar, pillarSpawnPos, Quaternion.identity).transform;

        newPillar.localScale = pillarScale;
        newPillar.parent = pillarFieldMeshes.transform;
    }

}
