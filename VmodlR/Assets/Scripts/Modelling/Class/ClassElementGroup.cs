using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassElementGroup : MonoBehaviour
{
    public List<ClassSideElement> classSideElements = new List<ClassSideElement>();
    public Transform elementContainer;

    public GameObject classElementPrefab;

    void Start()
    {
        if(elementContainer == null)
        {
            Debug.LogError($"Element Container is null on {gameObject.name}");
            this.enabled = false;
        }
    }

    public void LocalCreateElement(ClassContentSynchronizer masterSynchronizer, ClassSideMirror sideMirror, ClassElementType elementType, int elementID, int elementIndex)
    {
        //Instantiate the element 
        GameObject classElementGO = Instantiate(classElementPrefab, elementContainer);
        //Initialize the classElement & its synchronizer
        ClassSideElement classElement = classElementGO.GetComponent<ClassSideElement>();
        UpdateSiblingPositioning(classElement, elementIndex);//Move the element to the correct position under its container
        ClassElementSynchronizer elementSynchronizer = classElementGO.GetComponent<ClassElementSynchronizer>();
        classElement.Initialize(elementType, elementID, elementSynchronizer, masterSynchronizer);
        elementSynchronizer.Initialize(sideMirror, classElement);

        //Insert the new class element at the correct position
        classSideElements.InsertOrAppend(elementIndex, classElement);
    }

    public void LocalMoveElement(int elementID, int newIndex)
    {
        ClassSideElement moveElement = classSideElements.Find(element => element.ElementID == elementID);
        if(moveElement)
        {
            Debug.LogError($"tried to move element {elementID} to index {newIndex} but the element was not present on classSide {gameObject.name}");
            return;
        }
        //remove from previous position
        classSideElements.Remove(moveElement);
        //re-add at new position
        classSideElements.InsertOrAppend(newIndex, moveElement);
        //update sibling positioning
        UpdateSiblingPositioning(moveElement, newIndex);
    }

    public void LocalDeleteElement(int elementID)
    {
        ClassSideElement elementToDelete = classSideElements.Find(new System.Predicate<ClassSideElement>(element => element.ElementID == elementID));
        classSideElements.Remove(elementToDelete);
        Destroy(elementToDelete.gameObject);
    }

    public void LocalChangeValue(int elementID, string newValue)
    {
        ClassSideElement elementToChange = classSideElements.Find(new System.Predicate<ClassSideElement>(element => element.ElementID == elementID));
        elementToChange.LocalChangeValue(newValue);
    }

    

    private void UpdateSiblingPositioning(ClassSideElement element, int elementIndex)
    {
        //index 0 is always ocupied by the insert element panel, thus the index in the model (elementIndex) is always 1 smaller than the index in the siblings list
        element.transform.SetSiblingIndex(elementIndex + 1);
    }
}
