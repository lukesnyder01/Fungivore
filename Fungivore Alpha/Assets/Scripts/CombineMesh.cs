using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
{
    [Header("Mesh Combine Settings")]
    public bool destroyChildren = true;
    public bool meshIsLarge = true;


    public void MultiMaterialCombine(GameObject meshesToCombine)
    {
        //get array of all child meshes excluding inactive ones
        MeshFilter[] meshFilters = meshesToCombine.GetComponentsInChildren<MeshFilter>(false);

        //


    }



    public void Combine(GameObject meshesToCombine)
    {
        //keep track of the transform information of the parent of the mesh to combine
        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;

        //move the parent to the origin and zero its rotation and scale
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        //get array of all child meshes excluding inactive ones
        MeshFilter[] meshFilters = meshesToCombine.GetComponentsInChildren<MeshFilter>(false);

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

        Destroy(meshesToCombine.gameObject.GetComponent<MeshCollider>());

        for (int m = 0; m < meshFilters.Length; m++)
        {
            //ignore empty mesh filters
            if (meshFilters[m].sharedMesh == null) continue;

            combineInstances[m].mesh = meshFilters[m].sharedMesh;
            combineInstances[m].transform = meshFilters[m].transform.localToWorldMatrix;
            meshFilters[m].gameObject.SetActive(false);
        }



        var meshFilter = meshesToCombine.GetComponent<MeshFilter>();

        meshFilter.mesh = new Mesh();

        if (meshIsLarge)
        {
            meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        meshFilter.mesh.CombineMeshes(combineInstances, true);


        //these might not be necessary

        //meshFilter.mesh.RecalculateBounds();
        //meshFilter.mesh.RecalculateNormals();
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

        //move combined mesh back to original position
        transform.rotation = oldRotation;
        transform.position = oldPosition;
        transform.localScale = oldScale;
    }

}
