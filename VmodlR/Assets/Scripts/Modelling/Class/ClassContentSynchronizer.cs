using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(ClassSideMirror))]
public class ClassContentSynchronizer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static string currMaxIDKey = "CurrMaxID";
    public static string lastAddedIndexKey = "LastAddedIndex";
    public static string lastAddedElemTypeKey = "LastAddedElemType";

    public ClassSideMirror classSides;

    #region Private Fields

    private List<int> fieldsModel = new List<int>();
    private List<int> operationsModel = new List<int>();

    #endregion

    #region Monobehaviour Callbacks

    void Awake()
    {
        classSides = GetComponent<ClassSideMirror>();
        
        //Try to get the custom ID & spawn properties. Catch the case where these properties do not exist yet
        try
        {
            int currMaxID = (int)PhotonNetwork.CurrentRoom.CustomProperties[currMaxIDKey];
            int lastAddedIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties[lastAddedIndexKey];
            ClassElementType lastAddedType = (ClassElementType)PhotonNetwork.CurrentRoom.CustomProperties[lastAddedElemTypeKey];
        }
        catch(NullReferenceException)
        {
            //If the properties don't exist yet, create them with default values
            Hashtable initialProperties = new Hashtable();
            initialProperties[currMaxIDKey] = -1;
            initialProperties[lastAddedIndexKey] = -1;
            initialProperties[lastAddedElemTypeKey] = ClassElementType.Field;//This is just a default value, it does not actually matter, it just has to be set to some value.
            PhotonNetwork.CurrentRoom.SetCustomProperties(initialProperties);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            AddElementAt(0, ClassElementType.Field);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddElementAt(0, ClassElementType.Operation);
        }
    }

    /*
     * Add / Remove Callback Target is not needed when deriving from MonoBehaviourPunCallbacks, because MonoBehaviourPunCallbacks does that already

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    */

    #endregion

    #region Public Methods

    public void AddFieldBelowZero()
    {
        AddElementAt(0, ClassElementType.Field);
    }

    public void AddOperationBelowZero()
    {
        AddElementAt(0, ClassElementType.Operation);
    }

    public void AddElementBelow(int ElementID, ClassElementType newElementType)
    {
        switch(newElementType)
        {
            case ClassElementType.Field:
                //Index Plus one makes sure the new element is added below the Original Element with the given ElementID
                AddElementAt(fieldsModel.IndexOf(ElementID) + 1, newElementType);
                break;
            case ClassElementType.Operation:
                AddElementAt(operationsModel.IndexOf(ElementID) + 1, newElementType);
                break;
        }
    }

    public void AddElementAt(int insertAtIndex, ClassElementType newElementType)
    {
        //get the current MaxID, lastAddedIndex & lastAddedElementType
        Room room = PhotonNetwork.CurrentRoom;
        Hashtable hasttable = room.CustomProperties;
        int currMaxID = (int)hasttable[currMaxIDKey];
        int lastAddedIndex = (int)hasttable[lastAddedIndexKey];
        ClassElementType lastAddedType = (ClassElementType)hasttable[lastAddedElemTypeKey];

        //put those old property values in a hashtable, to perform a check-and-set with (See https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state#custom_properties)
        Hashtable expectedProperty = new Hashtable();
        expectedProperty[currMaxIDKey] = currMaxID;
        expectedProperty[lastAddedIndexKey] = lastAddedIndex;
        expectedProperty[lastAddedElemTypeKey] = lastAddedType;

        //Try to set the new property values by check-and-set, this will trigger a callback if it succeeds, which then spawns the new model element.
        //If not nothing happens. This will only fail if two clients spawn an element almost simultaneously
        int newID = currMaxID + 1;
        Hashtable updatedProperty = new Hashtable();
        updatedProperty[currMaxIDKey] = newID;
        updatedProperty[lastAddedIndexKey] = insertAtIndex;
        updatedProperty[lastAddedElemTypeKey] = newElementType;
        PhotonNetwork.CurrentRoom.SetCustomProperties(updatedProperty, expectedProperty);
    }

    public void DeleteElement(ClassElementType elementType, int elementID)
    {
        int[] updatedModel;
        switch (elementType)
        {
            case ClassElementType.Field:
                updatedModel = CalculateModelForRemovedElement(fieldsModel, elementID);
                RaiseUpdateClassContentEvent(elementType, updatedModel, fieldsModel);
                break;
            case ClassElementType.Operation:
                updatedModel = CalculateModelForRemovedElement(operationsModel, elementID);
                RaiseUpdateClassContentEvent(elementType, updatedModel, operationsModel);
                break;
        }
    }

    #endregion

    #region IOnEvent interface implementation

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == EventCodes.updateClassContent)
        {
            //extract the sent data from the event
            Hashtable eventData = (Hashtable)photonEvent.CustomData;
            int contentSynchronizerID = (int)eventData["ContentSynchronizerID"];
            //check if the class that the event wants to synchronize is this class
            if (contentSynchronizerID == photonView.ViewID)
            {
                int[] updatedElementIDs = (int[])eventData["UpdatedElementIDs"];
                ClassElementType elementsType = (ClassElementType)eventData["UpdatedElementsType"];
                UpdateElementExistence(updatedElementIDs, elementsType);
            }
        }
    }

    #endregion

    #region Pun InRoom Callbacks

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        int currMaxID = (int)propertiesThatChanged[currMaxIDKey];
        int lastAddedIndex = (int)propertiesThatChanged[lastAddedIndexKey];
        ClassElementType lastAddedElemType = (ClassElementType)propertiesThatChanged[lastAddedElemTypeKey];

        //Debug.Log($"Properties changed: ID: {currMaxID} Index: {lastAddedIndex}, Type: {lastAddedElemType}");
        
        if(currMaxID >= 0)
        {
            
            //Insert new element into synchronization model
            int finalIndex = AddElementToModel(lastAddedElemType, currMaxID, lastAddedIndex);
            //Debug.Log($"\nAdding new Class Element at finalIndex {finalIndex} with lastAddedIndex {lastAddedIndex}");
            //Debug.Log($"\nFieldsModel: {ArrayUtils.ArrayToString<int>(fieldsModel.ToArray())}");
            //Debug.Log($"\nOperationsModel: {ArrayUtils.ArrayToString<int>(operationsModel.ToArray())}");
            //Instruct class sides to add the new element to the actual GameObject 
            classSides.LocalCreateElement(this, lastAddedElemType, currMaxID, finalIndex);
        }
    }

    #endregion

    #region Private Methods

    private int[] CalculateModelForRemovedElement(List<int> model, int removeElementID)
    { 
        List<int> modelList = CloneModelList(model);

        modelList.Remove(removeElementID);//IDs are unique, so we can safely use Remove

        return modelList.ToArray();
    }

    private List<int> CloneModelList(List<int> model)
    {
        List<int> modelList = new List<int>();
        foreach (int elementID in model)
        {
            modelList.Add(elementID);
        }
        return modelList;
    }

    private int AddElementToModel(ClassElementType elementType, int elementID, int elementIndex)
    {
        switch(elementType)
        {
            case ClassElementType.Field:
                if(fieldsModel.Count -1 >= elementIndex)
                {
                    //if the index already exists, insert the new element
                    fieldsModel.Insert(elementIndex, elementID);
                    return elementIndex;
                }
                else
                {
                    //if the index does not exist, append the element to the end of the list and return its index
                    fieldsModel.Add(elementID);
                    return fieldsModel.Count - 1;
                }
            case ClassElementType.Operation:
                if (operationsModel.Count - 1 >= elementIndex)
                {
                    //if the index already exists, insert the new element
                    operationsModel.Insert(elementIndex, elementID);
                    return elementIndex;
                }
                else
                {
                    //if the index does not exist, append the element to the end of the list and return its index
                    operationsModel.Add(elementID);
                    return operationsModel.Count - 1;
                }
        }
        //This should never be called, linter falsly says that not all code path return a value
        return -1;
    }

    private void RaiseUpdateClassContentEvent(ClassElementType elementType, int[] updatedModel, List<int> originalModel)
    {
        Hashtable oldContent = new Hashtable();
        oldContent.Add("ContentSynchronizerID", photonView.ViewID);
        oldContent.Add("UpdatedElementIDs", originalModel.ToArray());//this is still the "old" model
        oldContent.Add("UpdatedElementsType", elementType);

        //delete the last change event from the room's event cache by filtering by its content
        RaiseEventOptions deleteEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        if (!PhotonNetwork.RaiseEvent(EventCodes.updateClassContent, oldContent, deleteEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not deleted!");
        }

        //create the new content for the new event
        Hashtable newConent = new Hashtable();
        newConent.Add("ContentSynchronizerID", photonView.ViewID);
        newConent.Add("UpdatedElementIDs", updatedModel);
        newConent.Add("UpdatedElementsType", elementType);

        //raise the new change event, that replaces the one we just deleted
        RaiseEventOptions createEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        if (!PhotonNetwork.RaiseEvent(EventCodes.updateClassContent, newConent, createEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not raised!");
        }
    }

    private void UpdateElementExistence(int[] updatedElements, ClassElementType elementType)
    {
        //Operate on the element model that corresponds to the given element type
        List<int> elementsModel = null;
        switch (elementType)
        {
            case ClassElementType.Field:
                elementsModel = fieldsModel;
                break;
            case ClassElementType.Operation:
                elementsModel = operationsModel;
                break;
        }

        int updatingElementIndex;

        for(updatingElementIndex = 0; updatingElementIndex < updatedElements.Length; updatingElementIndex++)
        {
            if (updatingElementIndex >= updatedElements.Length)
            {
                //skip to deleting everything with index >= updatedElements.Length
                RemoveLeftoverElements(updatingElementIndex, elementsModel, elementType);
                break;
            }
            else if(updatingElementIndex >= elementsModel.Count)
            {
                //add all new elements that are in updatedElements but not in elementsModel
                AddMissingElements(updatingElementIndex, updatedElements, elementsModel, elementType);
                break;
            }

            int newElement = updatedElements[updatingElementIndex];
            int oldElement = elementsModel[updatingElementIndex];

            if (newElement != oldElement) //something changed at this index
            {
                if(elementsModel.Contains(newElement))//the element was moved here from a different position
                {
                    //move newElement from old to new position
                    classSides.LocalMoveElement(elementType, newElement, updatingElementIndex);
                    elementsModel.InsertOrAppend(updatingElementIndex, newElement);
                }
                else //the element was added at this position
                {
                    //add newElement at the current index
                    classSides.LocalCreateElement(this, elementType, newElement, updatingElementIndex);
                    elementsModel.Insert(updatingElementIndex, newElement);
                }
            }
        }
    }

    private void AddMissingElements(int firstAddIndex, int[] updatedElements, List<int> elementsModel, ClassElementType elementType)
    {
        //Add all new elements that are not in the elementsModel yet
        for (int addElementIndex = firstAddIndex; addElementIndex < updatedElements.Length; addElementIndex++)
        {
            //Add the ui element on all classes
            classSides.LocalCreateElement(this, elementType, updatedElements[addElementIndex], addElementIndex);
            //add the element to the model
            elementsModel.Add(updatedElements[addElementIndex]);
        }
    }

    /// <summary>
    /// Removes the elements at the end of the elementsModel up to and including the element at the lastRemoveIndex
    /// </summary>
    /// <param name="lastRemoveIndex"></param>
    /// <param name=""></param>
    private void RemoveLeftoverElements(int lastRemoveIndex, List<int> elementsModel, ClassElementType elementType)
    {
        //we go over the list from back to front, so the element indicies in the model list don't shift during the for loop
        for (int deleteElementIndex = elementsModel.Count; deleteElementIndex >= lastRemoveIndex; deleteElementIndex--)
        {
            //delete element at deleteElementIndex, because it was not present in the updatedElements list.
            int elementToDelete = elementsModel[deleteElementIndex];
            //tell Class Side Mirror to delete the respective UI Element
            classSides.LocalDeleteElement(elementType, elementToDelete);
            //Delete the element from the ContentSynchronizer's internal model
            elementsModel.RemoveAt(deleteElementIndex);
        }
    }

    #endregion
}
