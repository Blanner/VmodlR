using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSideMirror : MonoBehaviour
{
    public List<ClassSide> mirroredSides = new List<ClassSide>();

    public void LocalChangeElement(ClassElementType elementType, int elementID, string newValue)
    {
        foreach(ClassSide side in mirroredSides)
        {
            side.LocalChangeElement(elementType, elementID, newValue);
        }
    }

    public void LocalCreateElement(ClassContentSynchronizer masterSynchronizer, ClassElementType elementType, int elementID, int elementIndex)
    {
        foreach(ClassSide side in mirroredSides)
        {
            side.LocalCreateElement(masterSynchronizer, this, elementType, elementID, elementIndex);
        }
    }

    public void LocalMoveElement(ClassElementType elementType, int elementID, int newIndex)
    {
        foreach(ClassSide side in mirroredSides)
        {
            side.LocalMoveElement(elementType, elementID, newIndex);
        }
    }

    public void LocalDeleteElement(ClassElementType elementType, int elementID)
    {
        foreach(ClassSide side in mirroredSides)
        {
            side.LocalDeleteElement(elementType, elementID);
        }
    }
}
