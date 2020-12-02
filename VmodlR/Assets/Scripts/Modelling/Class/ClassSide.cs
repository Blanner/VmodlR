using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(OVRRaycaster))]
public class ClassSide : MonoBehaviour
{
    public ClassElementGroup fields;
    public ClassElementGroup operations;

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
    }

}
