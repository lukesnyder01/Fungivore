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
    private InteractableObject currentTarget;
    private TextMeshPro textMesh;
    private PlayerInput playerInput;


    void Awake()
    {
        textMesh = gameObject.GetComponent<PlayerUI>().promptTextUI;
        cameraTransform = Camera.main.transform;
        textMesh.text = null;
        playerInput = GetComponent<PlayerInput>();
    }


    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * interactDist);


        if (Physics.Raycast(ray, out hitInfo, interactDist, mask))
        {
            if (hitInfo.transform.TryGetComponent(out InteractableObject interactable))
            {
                if (currentTarget == null)
                {
                    currentTarget = interactable;
                    currentTarget.StartFocus();
                    textMesh.text = interactable.promptText;
                }
                else
                {
                    textMesh.text = interactable.promptText;
                }
            }
            else //didn't hit an interactable
            {
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
        }
    }
}
