using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateJoints : MonoBehaviour
{
    public float jointMass = 0.5f;
    public float drag = 0.05f;
    public float angularDrag = 0.05f;
    public bool useGravity = false;



    [SerializeField]
    private List<GameObject> allBones;

    [ContextMenu("GenerateJoints")]
    private void GenerateJoints()
    {
        allBones = new List<GameObject>();

        GetBone(transform);

        Rigidbody previousRB = null;

        for (int i = 0; i < allBones.Count; i++)
        {
            allBones[i].AddComponent<CharacterJoint>();
            allBones[i].GetComponent<CharacterJoint>().connectedBody = previousRB;

            previousRB = allBones[i].GetComponent<Rigidbody>();


        }

    }

    [ContextMenu("ConfigureJoints")]
    private void ConfigureJoints()
    {
        allBones = new List<GameObject>();

        GetBone(transform);

        for (int i = 0; i < allBones.Count; i++)
        {
            GameObject currentBone = allBones[i];

            currentBone.GetComponent<Rigidbody>().mass = jointMass;
            currentBone.GetComponent<Rigidbody>().useGravity = useGravity;
        }

    }


    private void GetBone(Transform bone)
    {
        allBones.Add(bone.gameObject);

        foreach (Transform child in bone)
        {
            GetBone(child);
        }
    }

}
