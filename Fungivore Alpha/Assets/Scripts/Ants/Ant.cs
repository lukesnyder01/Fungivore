using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Ant Settings")]
    public int minSteps = 20;
    public int maxSteps = 25;
    public float waitTime = 0f;

    [HideInInspector]
    public int totalSteps;
    [HideInInspector]
    public int currentStep;

    public bool multipleRespawns;
    public int respawnCount;

    public int antSideLength = 1;



    public int verticalSpawnOffset;

    [HideInInspector]
    public Vector3 boxCenter;
    [HideInInspector]
    public Vector3 halfExtents;
    [HideInInspector]
    public Vector3 boxRotation;
    [HideInInspector]
    public float antMoveDistance;
    [HideInInspector]
    public float minPlayerDistance;

    public float minPlayerDistancePad = 1f;

    [HideInInspector]
    public float playerDistance;


    [Header("Object References")]
    public GameObject respawnPrefab;

    public GameObject player;

    public GameObject[] spawnPrefabs;

    [HideInInspector]
    public List<Vector3> availablePositions = new List<Vector3>();
    [HideInInspector]
    public List<int> availableRotations = new List<int>();
    [HideInInspector]
    public bool noMovesLeft = false;


    [Header("Mesh Combine Settings")]
    public bool destroyChildren = true;
    public bool meshIsLarge = false;


    public RaycastHit hit;

    [HideInInspector]
    public Vector3 antPos;

    [HideInInspector]
    public int antDir;

    [HideInInspector]
    public Vector3[] directionArray = new[] {
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 0f, -1f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, 0f, 1f)
    };


    public virtual void Start()
    {
        InitializeParameters();
        //_antList.Add(gameObject);
        StartCoroutine(IterateGrowth());
    }


    public virtual void InitializeParameters()
    {
        antPos = transform.position + new Vector3(0, verticalSpawnOffset, 0);

        //set antDir to object direction
        antDir = Mathf.RoundToInt(transform.rotation.y / 90) + 3 % 4;

        //set hitbox half extents
        var halfAntSide = antSideLength / 2;
        halfExtents = new Vector3(halfAntSide, halfAntSide, halfAntSide);


        player = GameObject.Find("Player");

        antMoveDistance = antSideLength;

        minPlayerDistance = antSideLength + minPlayerDistancePad;

        Random.InitState(transform.position.GetHashCode() % 100000);
        totalSteps = Random.Range(minSteps, maxSteps);
    }


    public IEnumerator IterateGrowth()
    {
        for (int i = 0; i < totalSteps + 1; i++) 
        {
            yield return new WaitForSeconds(waitTime);
            yield return new WaitForFixedUpdate();

            Grow();
        }
    }

    public virtual void Grow()
    {
        if (currentStep < totalSteps)
        {
            currentStep++;

            playerDistance = (antPos - player.transform.position).sqrMagnitude;

            if (playerDistance > (minPlayerDistance * minPlayerDistance))
            {
                AddNewBlock();
            }
        }
        else
        {
            if (multipleRespawns && noMovesLeft == false)
            {
                Respawn();
                EndAnt();
            }

            EndAnt();
        }
    }


    public virtual void AddNewBlock()
    {
        int selectedMove = 0;
        availablePositions.Clear();
        availableRotations.Clear();

        int straight = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;

        if (SpaceIsEmpty(directionArray[straight]))
        {
            availablePositions.Add(antPos + directionArray[straight] * antMoveDistance);
            availableRotations.Add(straight);
        }

        if (SpaceIsEmpty(directionArray[right]))
        {
            availablePositions.Add(antPos + directionArray[right] * antMoveDistance);
            availableRotations.Add(right);
        }

        if (SpaceIsEmpty(directionArray[left]))
        {
            availablePositions.Add(antPos + directionArray[left] * antMoveDistance);
            availableRotations.Add(left);
        }

        if (SpaceIsEmpty(Vector3.up))
        {
            availablePositions.Add(antPos + Vector3.up * antMoveDistance);
            availableRotations.Add(straight);
        }

        if (SpaceIsEmpty(-Vector3.up))
        {
            availablePositions.Add(antPos + -Vector3.up * antMoveDistance);
            availableRotations.Add(straight);
        }


        if (availablePositions.Count != 0)
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            selectedMove = Random.Range(0, availablePositions.Count);

            GameObject randomPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            Vector3 randomRotation = directionArray[Random.Range(0, directionArray.Length)];


            GameObject newBlock = Instantiate(randomPrefab, antPos, Quaternion.identity);
            newBlock.transform.forward = randomRotation;

            newBlock.transform.SetParent(this.transform);

            antPos = availablePositions[selectedMove];
            antDir = availableRotations[selectedMove];
        }
        else
        {
            EndAnt();
        }
    }


    public virtual bool SpaceIsEmpty(Vector3 direction)
    {
        return (!Physics.BoxCast(antPos, halfExtents, direction, out hit, Quaternion.identity, antMoveDistance));
    }




    public virtual void Respawn()
    {
        Instantiate(respawnPrefab, antPos, transform.rotation);
    }


    public void EndAnt()
    {
        CombineMesh(gameObject);
    }


    public void CombineMesh(GameObject meshesToCombine)
    {
        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;


        MeshFilter[] meshFilters = meshesToCombine.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        Destroy(meshesToCombine.gameObject.GetComponent<MeshCollider>());

        //Matrix4x4 myTransform = meshesToCombine.transform.worldToLocalMatrix;

        for (int m = 0; m < meshFilters.Length; m++)
        {
            if (meshFilters[m].sharedMesh == null) continue;

            combine[m].mesh = meshFilters[m].sharedMesh;

            //combine[m].transform = myTransform * meshFilters[m].transform.localToWorldMatrix;

            combine[m].transform = meshFilters[m].transform.localToWorldMatrix;
            meshFilters[m].gameObject.SetActive(false);
        }


        var meshFilter = meshesToCombine.GetComponent<MeshFilter>();

        


        meshFilter.mesh = new Mesh();

        if (meshIsLarge)
        {
            meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        meshFilter.mesh.CombineMeshes(combine, true);
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.Optimize();

        Mesh newMeshCollider = new Mesh();

        newMeshCollider.indexFormat = meshFilter.mesh.indexFormat;
        newMeshCollider.vertices = meshFilter.mesh.vertices;
        newMeshCollider.triangles = meshFilter.mesh.triangles;

        MeshCollider meshCollider = meshesToCombine.gameObject.AddComponent<MeshCollider>();

        meshCollider.sharedMesh = newMeshCollider;

        if (!destroyChildren)
        {
            meshesToCombine.gameObject.transform.DetachChildren();
        }
        else
        {
            foreach (Transform child in meshesToCombine.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }



        meshesToCombine.gameObject.SetActive(true);

        transform.rotation = oldRotation;
        transform.position = oldPosition;
        transform.localScale = oldScale;

    }

}