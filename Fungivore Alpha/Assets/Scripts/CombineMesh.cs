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


    public void MultiMaterialCombine(GameObject combineParent)
    {

        //make a list of materials, we'll make a combine instance for each one
        List<Material> materials = new List<Material>();

        //make a list of meshes, one for each material
        List<GameObject> perMatCombineParents = new List<GameObject>();


        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false);

        //parent each mesh to combine to their corresponding perMaterialMeshes
        for (int i = 0; i < renderers.Length; i++)
        {
            //get the first material of this renderer
            Material localMat = renderers[i].sharedMaterials[0];

            if (localMat)
            {
                //if the material is on the materials list, add the gameObject to the appropriate perMaterialMesh
                if (materials.Contains(localMat))
                {
                    var matIndex = materials.IndexOf(localMat);
                    renderers[i].gameObject.transform.parent = perMatCombineParents[matIndex].transform;
                }
                else //create a new perMaterialMesh and add the gameObject
                {
                    materials.Add(localMat);
                    perMatCombineParents.Add(renderers[i].gameObject);
                }
            }

        }

        for (int i = 0; i < perMatCombineParents.Count; i++)
        {
            
            Combine(perMatCombineParents[i]);
            if (i > 0)
            {
                perMatCombineParents[i].transform.parent = perMatCombineParents[i - 1].transform;
            }

            Debug.Log("mesh material is: " + perMatCombineParents[i].GetComponent<MeshRenderer>().sharedMaterials[0]);
        }


    }

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


        //these might not be necessary
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.Optimize();

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
            //Debug.Log("added a mesh collider to " + combineParent);
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
    }

}
