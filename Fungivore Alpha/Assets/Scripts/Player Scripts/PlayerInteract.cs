using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDist = 1.5f;

    [SerializeField]
    LayerMask mask = 0;

    private Transform cameraTransform;
    private IInteractable currentTarget;
    private TextMeshPro textMesh;
    private PlayerInput playerInput;
    private TextToSpeech textToSpeech;

    public GameObject testCubePrefab;


    void Awake()
    {
        textMesh = gameObject.GetComponent<PlayerUI>().promptTextUI;
        cameraTransform = Camera.main.transform;
        textMesh.text = null;
        playerInput = GetComponent<PlayerInput>();
        textToSpeech = GameObject.Find("AudioManager").GetComponent<TextToSpeech>();
    }


    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * interactDist);


        if (Physics.Raycast(ray, out hitInfo, interactDist, mask))
        {
            if (hitInfo.transform.TryGetComponent(out IInteractable interactable))
            {
                if (currentTarget == null)
                {
                    currentTarget = interactable;
                    currentTarget.StartFocus();
                    textMesh.text = interactable.PromptText;
                }
                else
                {
                    textMesh.text = interactable.PromptText;
                }
            }
            else //didn't hit an interactable
            {
                if (hitInfo.transform.CompareTag("Solid Block") && Input.GetKeyDown(KeyCode.F))
                {
                    var hitPoint = hitInfo.point;
                    var hitNormal = hitInfo.normal;

                    Vector3 hitCubePos = hitPoint - hitNormal * 0.5f; // Move the hit point inside the cube

                    hitCubePos.x = Mathf.FloorToInt(hitCubePos.x);
                    hitCubePos.y = Mathf.FloorToInt(hitCubePos.y);
                    hitCubePos.z = Mathf.FloorToInt(hitCubePos.z);

                    Vector3 targetCubePos = hitCubePos + hitNormal;
                    
                    Chunk chunk = World.Instance.GetChunkAt(targetCubePos);
                    
                    chunk.SetBlock(targetCubePos, Voxel.VoxelType.Stone);
                }

                if (currentTarget != null)
                {
                    currentTarget.LoseFocus();
                    currentTarget = null;
                }

                textMesh.text = null;
            }
        }
        else //if raycast didn't hit anything at all
        {
            if (currentTarget != null)
            {
                currentTarget.LoseFocus();
                currentTarget = null;
            }

            textMesh.text = null;
        }


        //interact if there's an interactable target
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTarget != null)
            {
                currentTarget.Interact();
            }
            else
            {
                // if the player isn't looking at an interactable, the E button
                // will stop the current speaker and show the full text
                if (textToSpeech.textIsHidden == false)
                {
                    textToSpeech.CancelReadingAndDisplayFullText();
                }
            }
        }
    }
}
