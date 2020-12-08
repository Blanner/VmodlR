using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToggleCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsPointerInside { get; private set; }

    private HoverToggle notifyToggle;

    private void Awake()
    {
        IsPointerInside = false;
    }

    public void Initialize(HoverToggle notifyToggle)
    {
        this.notifyToggle = notifyToggle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"\nThe cursor entered the selectable UI element on GO {eventData.pointerEnter} with ID {eventData.pointerId}");
        IsPointerInside = true;
        notifyToggle.UpdateToggle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"\nThe cursor exited the selectable UI element on GO {eventData.pointerEnter} with ID {eventData.pointerId}");
        IsPointerInside = false;
        notifyToggle.UpdateToggle();
    }

}
