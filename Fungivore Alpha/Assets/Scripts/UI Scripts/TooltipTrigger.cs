using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 position;
    public string content;
    public string header;
    public float verticalOffset = 10f;

    public bool pointerIsInside;


    void OnDisable()
    {
        pointerIsInside = false;
    }


    public void Update()
    {
        if (pointerIsInside)
        {
            position = new Vector2(transform.position.x, transform.position.y - verticalOffset);
            TooltipSystem.Show(position, content, header);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        position = new Vector2(transform.position.x, transform.position.y - verticalOffset);
        TooltipSystem.Show(position, content, header);

        pointerIsInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsInside = false;
        TooltipSystem.Hide();
    }


}
