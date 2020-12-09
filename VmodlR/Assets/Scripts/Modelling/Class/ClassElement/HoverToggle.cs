using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverToggle : MonoBehaviour
{
    public GameObject toggleTarget;

    private HoverToggleCollider[] uiColliders;
    private bool IsPointerInsideCollider = false;

    public void Awake()
    {
        if(toggleTarget == null)
        {
            Debug.LogError($"ToggleTarget was null on {gameObject.name}");
            gameObject.SetActive(false);
        }
        else
        {
            toggleTarget.SetActive(false);
        }

        uiColliders = GetComponentsInChildren<HoverToggleCollider>();
        foreach (HoverToggleCollider collider in uiColliders)
        {
            collider.Initialize(this);
        }
        //Debug.Log($"Found {uiColliders.Length} HoverToggleColliders in children of GO {gameObject}");
    }

    public void UpdateToggle()
    {
        IsPointerInsideCollider = false;
        foreach(HoverToggleCollider collider in uiColliders)
        {
            if(collider.IsPointerInside)
            {
                IsPointerInsideCollider = true;
            }
        }

        if(IsPointerInsideCollider)
        {
            toggleTarget.SetActive(true);
        }
        else
        {
            toggleTarget.SetActive(false);
        }
    }
}
