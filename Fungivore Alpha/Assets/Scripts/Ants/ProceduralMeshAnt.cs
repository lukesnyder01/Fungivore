using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshAnt : MonoBehaviour
{
    public float cubeSize = 1.0f;
    public int maxCubes = 100;

    public float timeBetweenSteps = 1f;

    public float stepTimer = 1f;

    public Vector3[] directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };



    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private Mesh mesh;
    private MeshCollider meshCollider;

    public Vector3 antPosition;
    private int cubeCount = 0;




    void Awake()
    {
        stepTimer = timeBetweenSteps;
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        antPosition = transform.position;
    }

    void Start()
    {
        AddCube(antPosition);
    }


    void Update()
    {
        if (cubeCount >= maxCubes)
        {
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            stepTimer = timeBetweenSteps;

            TryToMoveAnt();
        }
    }

    private void TryToMoveAnt()
    {
        // Check for adjacent empty space
        Vector3 newPos = antPosition + RandomDirection() * cubeSize;
        if (!IsDirectionEmpty(newPos))
        {
            return;
        }

        // Move ant to new position
        antPosition = newPos;

        // Add new cube to mesh
        AddCube(antPosition);

        UpdateMeshAndCollider();

    }


    private Vector3 RandomDirection()
    {
        return directions[Random.Range(0, directions.Length)];
    }


    private bool IsDirectionEmpty(Vector3 direction)
    {
        // Define the size and distance of the box to cast
        float boxSize = 0.9f;
        float distance = cubeSize * 0.5f;

        // Cast a box in the specified direction and check for collisions
        RaycastHit hit;
        bool hasHit = Physics.BoxCast(antPosition, new Vector3(boxSize, boxSize, boxSize), direction, out hit, Quaternion.identity, distance);

        // If there are no collisions, the direction is empty
        return !hasHit;
    }


    void AddCube(Vector3 position)
    {
        // Add vertices
        int startIndex = vertices.Count;

        vertices.Add(position + new Vector3(-cubeSize, -cubeSize, -cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(cubeSize, -cubeSize, -cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(cubeSize, -cubeSize, cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(-cubeSize, -cubeSize, cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(-cubeSize, cubeSize, -cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(cubeSize, cubeSize, -cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(cubeSize, cubeSize, cubeSize) * 0.5f);
        vertices.Add(position + new Vector3(-cubeSize, cubeSize, cubeSize) * 0.5f);

        // Add triangles
        triangles.Add(startIndex + 0);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 0);

        triangles.Add(startIndex + 5);
        triangles.Add(startIndex + 4);
        triangles.Add(startIndex + 7);
        triangles.Add(startIndex + 7);
        triangles.Add(startIndex + 6);
        triangles.Add(startIndex + 5);

        triangles.Add(startIndex + 4);
        triangles.Add(startIndex + 0);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 7);
        triangles.Add(startIndex + 4);

        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 5);
        triangles.Add(startIndex + 6);
        triangles.Add(startIndex + 6);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 1);

        triangles.Add(startIndex + 4);
        triangles.Add(startIndex + 5);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 0);
        triangles.Add(startIndex + 4);

        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 6);
        triangles.Add(startIndex + 6);
        triangles.Add(startIndex + 7);
        triangles.Add(startIndex + 3);

        // Add UVs
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        // Increase cube count
        cubeCount++;


    }

    private void UpdateMeshAndCollider() 
    {
        // Update mesh
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        // Update mesh collider
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }



}
