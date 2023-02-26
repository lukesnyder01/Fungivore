using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator3D : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public int cellSize;

    public GameObject cellPrefab;

    // Create the 3D array to store the state and directions for each cell
    int[,,] mazeGrid;



    // Start is called before the first frame update
    void Start()
    {
        mazeGrid = new int[sizeX, sizeY, sizeZ];

        //overlap box each cell to see if it's obstructed, then mark it 0 for empty and 1 for obstructed
        InitializeCells();

        //spawn a block in each empty cell
        StartCoroutine(SpawnCellPrefabs());



    }


    void InitializeCells()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var cellPosX = transform.position.x + (x * cellSize);
                    var cellPosY = transform.position.y + (y * cellSize);
                    var cellPosZ = transform.position.z + (z * cellSize);

                    if (SpaceIsEmpty(cellPosX, cellPosY, cellPosZ))
                    {
                        mazeGrid[x, y, z] = 0;
                    }
                    else
                    {
                        mazeGrid[x, y, z] = 1;
                    }
                }
            }
        }
    }



    IEnumerator SpawnCellPrefabs()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (mazeGrid[x, y, z] == 0) //spawn a block if the cell is marked 0 for empty
                    {
                        var cellPosX = transform.position.x + (x * cellSize);
                        var cellPosY = transform.position.y + (y * cellSize);
                        var cellPosZ = transform.position.z + (z * cellSize);

                        Transform newCell = Instantiate(cellPrefab, new Vector3(cellPosX, cellPosY, cellPosZ), Quaternion.identity).transform;

                        for (int i = 0; i < newCell.childCount; i++)
                        {
                            if (Random.value > 0.8)
                            {
                                newCell.GetChild(i).gameObject.SetActive(true);
                            }

                        }
                    }

                }

                yield return null;

            }
        }
    }


    bool SpaceIsEmpty(float x, float y, float z)
    {
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(cellSize, cellSize, cellSize) / 2 , Quaternion.identity);
        if (hitColliders.Length < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



}
