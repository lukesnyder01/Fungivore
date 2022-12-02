using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
    [Header("Mesh Combine Settings")]
    public bool destroyChildren = true;
    public bool meshIsLarge = true;


    public void Combine(GameObject meshesToCombine)
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
