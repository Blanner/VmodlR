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
        //Instantiate the element at the correct position under its container
        GameObject classElementGO = Instantiate(classElementPrefab, elementContainer);
        classElementGO.transform.SetSiblingIndex(elementIndex + 1);//index 0 is always ocupied by the insert element panel, thus the index in the model (elementIndex) is always 1 smaller than the index in the siblings list

        //Initialize the classElement & its synchronizer
        ClassSideElement classElement = classElementGO.GetComponent<ClassSideElement>();
        ClassElementSynchronizer elementSynchronizer = classElementGO.GetComponent<ClassElementSynchronizer>();
        classElement.Initialize(elementType, elementID, elementSynchronizer, masterSynchronizer);
        elementSynchronizer.Initialize(sideMirror, classElement);

        //Insert the new class element at the correct position
        if (classSideElements.Count - 1 >= elementIndex)
        {
            classSideElements.Insert(elementIndex, classElement);
        }
        else
        {
            classSideElements.Add(classElement);
        }
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
}
