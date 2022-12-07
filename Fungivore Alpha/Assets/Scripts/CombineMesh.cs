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

        //get all child meshes
        MeshFilter[] meshFilters = meshesToCombine.GetComponentsInChildren<MeshFilter>(false);

        List<Material> materials = new List<Material>();

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false);


        //make a list of meshes, one for each material
        List<GameObject> perMaterialMeshes = new List<GameObject>();

        //parent each mesh to combine to their corresponding perMaterialMeshes
        for (int i = 0; i < renderers.Length; i++)
        {
            //get the first material of this renderer
            Material localMat = renderers[i].sharedMaterials[0];

            //if the material is on the materials list, add the gameObject to the appropriate perMaterialMesh
            if (materials.Contains(localMat))
            {
                var matIndex = materials.IndexOf(localMat);
                renderers[i].gameObject.transform.parent = perMaterialMeshes[matIndex].transform;
            }
            else //create a new perMaterialMesh and add the gameObject
            {
                materials.Add(localMat);
                perMaterialMeshes.Add(renderers[i].gameObject);
            }
        }

        foreach (GameObject perMaterialMesh in perMaterialMeshes)
        {
            Debug.Log(perMaterialMesh.transform.parent);
            perMaterialMesh.transform.parent = null;

            Combine(perMaterialMesh);
            Debug.Log(perMaterialMesh.GetComponent<MeshRenderer>().sharedMaterials[0]);
        }

    }

    public void Combine(GameObject meshesToCombine)
    {
        //temporarily reset object transform to make math easier
        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;


        //get all mesh filters
        MeshFilter[] meshFilters = meshesToCombine.GetComponentsInChildren<MeshFilter>(false);
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];


        //remove parent object collider, we'll rebuild it later once the meshes are combined
        Destroy(meshesToCombine.gameObject.GetComponent<MeshCollider>());


        int i = 0;
        while (i < meshFilters.Length)
        {

            if (meshFilters[i].sharedMesh == null) continue;

            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }



        var meshFilter = meshesToCombine.GetComponent<MeshFilter>();

        meshFilter.mesh = new Mesh();

        //make the mesh index format 32 bit if there is a lot of geometry
        if (meshIsLarge)
        {
            meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }



        meshFilter.mesh.CombineMeshes(combineInstances, true, true);



        //these might not be necessary
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.Optimize();


        //add new mesh collider and give it the new combined mesh
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

        //reset combined mesh transform
        transform.rotation = oldRotation;
        transform.position = oldPosition;
        transform.localScale = oldScale;
    }

}
