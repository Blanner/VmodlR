using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;


/// <summary>
/// This Class handles interactions with the Class' name UI Element of one of the Class cube's side.
/// Changes are forwarded to the SideMirror mich contains the underlying model and synchronizes it across the network.
/// </summary>
public class ClassSideElement : MonoBehaviour//Pun
{

    public ClassElementSynchronizer elementSynchronizer;
    public ClassContentSynchronizer masterSynchronizer;

    public InputField elementInputField;

    //public int ElementIndex { get; private set; }
    public int ElementID { get; private set; }
    public string Value { get { return elementInputField.text; } }

    public ClassElementType ElementType { get; private set; }

    public void Awake()
    {
        if(elementInputField == null)
        {
            Debug.LogError($"Input Field for Class Side Element is null on {gameObject.name}!");
        }
    }

    public void Initialize(ClassElementType elementType, int elementID, ClassElementSynchronizer elementSynchronizer, ClassContentSynchronizer masterSynchronizer)
    {
        this.ElementType = elementType;
        this.ElementID = elementID;
        this.elementSynchronizer = elementSynchronizer;
        this.masterSynchronizer = masterSynchronizer;
    }

    public void RemoteAddElementBelow()
    {
        masterSynchronizer.AddElementBelow(ElementID, ElementType);
    }

    /// <summary>
    /// Notifies the mirror that the user has changed the class name.
    /// Call this in the OnValueChanged from the name input field of the class side
    /// </summary>
    /// <param name="newValue"></param>
    public void RemoteChangeValue(string newValue)
    {
        Debug.Log("\nElement: Remote Change Value");
        elementSynchronizer.RemoteChangeValue(newValue);
    }

    /// <summary>
    /// Changes the name without notifying the class side mirror.
    /// Call this from the mirror on each class side, to synchronize their values
    /// </summary>
    /// <param name="newValue"></param>
    public void LocalChangeValue(string newValue)
    {
        elementInputField.SetTextWithoutNotify(newValue);
        elementSynchronizer.LocalChangeValue(newValue);
    }

    /// <summary>
    /// Forwards a deletion order to the ClassContentSynchronizer to delete this element synchronously.
    /// This does not delete the element immediatly, it just initiates the process of synchronious deletion
    /// </summary>
    public void RemoteDeleteElement()
    {
        masterSynchronizer.DeleteElement(ElementType, ElementID);
    }
}
