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


    public void Combine(GameObject combineParent)
    {
        //store parent object transform
        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;


        //temporarily reset object transform to make math easier
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;


        //get all mesh filters
        MeshFilter[] meshFilters = combineParent.GetComponentsInChildren<MeshFilter>(false);
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];


        //get a reference to the combine parent collider
        Collider collider = combineParent.gameObject.GetComponent<Collider>();

        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].sharedMesh == null) continue;

            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        var meshFilter = combineParent.GetComponent<MeshFilter>();

        meshFilter.mesh = new Mesh();

        //make the mesh index format 32 bit if there is a lot of geometry
        if (meshIsLarge)
        {
            meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        //combine the meshes
        meshFilter.mesh.CombineMeshes(combineInstances, true, true);




        //if the combine parent had a collider, update it to the new mesh
        if (collider)
        {
            //remove parent mesh collider, we'll rebuild it later once the meshes are combined
            Destroy(combineParent.gameObject.GetComponent<Collider>());

            //create new mesh collider and give it the new combined mesh
            Mesh newMeshCollider = new Mesh();

            newMeshCollider.indexFormat = meshFilter.mesh.indexFormat;
            newMeshCollider.vertices = meshFilter.mesh.vertices;
            newMeshCollider.triangles = meshFilter.mesh.triangles;

            MeshCollider meshCollider = combineParent.gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = newMeshCollider;
        }


        if (!destroyChildren)
        {
           combineParent.gameObject.transform.DetachChildren();
        }
        else
        {
            foreach (Transform child in combineParent.transform)
            {

                GameObject.Destroy(child.gameObject);
            }
        }

        combineParent.gameObject.SetActive(true);

        //reset combined mesh transform
        transform.rotation = oldRotation;
        transform.position = oldPosition;
        transform.localScale = oldScale;

        //these might not be necessary
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.Optimize();
    }

}
