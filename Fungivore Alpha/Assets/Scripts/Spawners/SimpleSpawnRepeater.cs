using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SimpleSpawnRepeater : MonoBehaviour
{

    [Header("Growth Settings")]
    [SerializeField] private int minSteps = 20;
    [SerializeField] private int maxSteps = 20;
    public Vector3 moveDirection = new Vector3(0, 1, 0);
    public float moveDistance = 5f;
    [SerializeField] private float timeBetweenSteps = 0.5f;


    [Header("Object References")]

    public GameObject spawnPrefab;
    private GameObject player;

    [Header("Ant Settings")]

    private float playerDistance;
    private int randomSize = 0;
    private Vector3 antPos;

    void Start()
    {
        antPos = transform.position;
        player = GameObject.Find("Player");
        randomSize = Random.Range(minSteps, maxSteps);
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenSteps);

        for (int i = 0; i < randomSize; i++)
        {
            GameObject spawnedObject = Instantiate(spawnPrefab, antPos, transform.rotation);

            antPos = antPos + moveDirection * moveDistance;

            yield return wait;
        }

    }

}