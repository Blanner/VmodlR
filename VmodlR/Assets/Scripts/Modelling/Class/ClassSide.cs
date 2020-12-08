using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(OVRRaycaster))]
public class ClassSide : MonoBehaviour
{
    public ClassElementGroup fields;
    public ClassElementGroup operations;
    public VerticalLayoutGroup bodyVerticalLayoutGroup;

    private void Awake()
    {
        if(bodyVerticalLayoutGroup == null)
        {
            Debug.LogError($"Class Sides's Body Vertical Layout Group is not assigned on {gameObject.name}!");
        }
        if(operations == null)
        {
            Debug.LogError($"Operations Element Group is not assigned on {gameObject.name}!");
        }
        if(fields == null)
        {
            Debug.LogError($"Fields Element Group is not assigned on {gameObject.name}!");
        }
    }

    public void LocalChangeElement(ClassElementType elementType, int elementID, string newValue)
    {
        switch(elementType)
        {
            case ClassElementType.Field:
                fields.LocalChangeValue(elementID, newValue);
                break;
            case ClassElementType.Operation:
                operations.LocalChangeValue(elementID, newValue);
                break;
        }
    }

    public void LocalCreateElement(ClassContentSynchronizer masterSynchronizer, ClassSideMirror sideMirror, ClassElementType elementType, int elementID, int elementIndex)
    {
        switch (elementType)
        {
            case ClassElementType.Field:
                fields.LocalCreateElement(masterSynchronizer, sideMirror, elementType, elementID, elementIndex);
                break;
            case ClassElementType.Operation:
                operations.LocalCreateElement(masterSynchronizer, sideMirror, elementType, elementID, elementIndex);
                break;
        }
        UpdateElementLayout();
    }

    public void LocalDeleteElement(ClassElementType elementType, int elementID)
    {
        switch (elementType)
        {
            case ClassElementType.Field:
                fields.LocalDeleteElement(elementID);
                break;
            case ClassElementType.Operation:
                operations.LocalDeleteElement(elementID);
                break;
        }
        UpdateElementLayout();
    }

    private void UpdateElementLayout()
    {
        StartCoroutine(CoroutineUpdateLayout());
    }

    /// <summary>
    /// This Coroutine waits .01 seconds an then updates the vertical layoutgroup associated with the ClassSide's body.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineUpdateLayout()
    {
        yield return new WaitForSeconds(.01f);
        bodyVerticalLayoutGroup.CalculateLayoutInputVertical();
        bodyVerticalLayoutGroup.SetLayoutVertical();
    }
}
