using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombineMesh))]
public class Ant : MonoBehaviour
{
    [HideInInspector]
    public CombineMesh combineMesh;

    private bool antHasEnded = false;

    public int currentStep = 0;
    public int totalSteps;


    [Header("Ant Settings")]
    public int minSteps = 20;
    public int maxSteps = 25;
    public float waitTime = 0f;

    public int respawnCount;

    public int antSideLength = 1;


    public int verticalSpawnOffset;


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

    private Coroutine iterateGrowthCoroutine;


    public virtual void Awake()
    {
        InitializeParameters();
    }


    private void OnEnable()
    {
        if (!antHasEnded)
        {
            iterateGrowthCoroutine = StartCoroutine(IterateGrowth());
        }
    }


    private void OnDisable()
    {
        if (iterateGrowthCoroutine != null)
        {
            StopCoroutine(iterateGrowthCoroutine);
        }
    }



    public virtual void InitializeParameters()
    {
        combineMesh = GetComponent<CombineMesh>();

        antPos = transform.position + new Vector3(0, verticalSpawnOffset, 0);

        //set antDir to object direction
        antDir = Mathf.RoundToInt(transform.rotation.y / 90) + 3 % 4;



        player = GameObject.Find("Player");

        antMoveDistance = antSideLength;

        minPlayerDistance = antSideLength + minPlayerDistancePad;

        Random.InitState(transform.position.GetHashCode() % 100000);
        totalSteps = Random.Range(minSteps, maxSteps);
    }


    public IEnumerator IterateGrowth()
    {
        for (int i = currentStep; i < totalSteps; i++)
        {
            yield return new WaitForSeconds(waitTime);
            yield return new WaitForFixedUpdate();

            if (noMovesLeft == true)
            {
                EndAnt();
            }

            Grow();
        }


        if (currentStep >= totalSteps)
        {
            Debug.Log("currentStep >= totalSteps");
            EndAnt();
            yield return new WaitForFixedUpdate();
        }

    }


    public virtual void Grow()
    {
        currentStep++;

        playerDistance = (antPos - player.transform.position).sqrMagnitude;

        if (playerDistance > (minPlayerDistance * minPlayerDistance))
        {
            AddNewBlock();
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


        if (availablePositions.Count > 0)
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
            noMovesLeft = true;
            return;
        }
    }

    /*
    public virtual bool SpaceIsEmpty(Vector3 direction)
    {
        return (!Physics.BoxCast(antPos, halfExtents, direction, out hit, Quaternion.identity, antMoveDistance));
    }

    public virtual bool SpaceIsEmpty(Vector3 direction)
    {
        Collider[] hitcolliders = Physics.OverlapBox(antPos + direction, halfExtents, Quaternion.identity);

        if (hitcolliders.Length == 0)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    */

    public virtual bool SpaceIsEmpty(Vector3 direction)
    {
        return (!Physics.SphereCast(antPos, (antSideLength / 2) - 0.1f, direction, out hit, antMoveDistance));
    }


    public virtual void Respawn()
    {
        if (respawnCount > 0)
        {
            var newAnt = Instantiate(respawnPrefab, antPos, transform.rotation);
            if (newAnt.TryGetComponent(out AntRespawner respawner))
            {
                respawner.respawnsLeft = respawnCount - 1;
            }
            newAnt.transform.parent = transform.parent;
        }
    }


    public virtual void EndAnt()
    {
        antHasEnded = true;

        Debug.Log("Ended Ant");

        if (noMovesLeft == false)
        {
            Respawn();
        }

        combineMesh.Combine(gameObject);
    }


}