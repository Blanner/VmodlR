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

    public ClassSideMirror classSides;
    public GameObject fieldsModelCotainer;
    public GameObject operationsModelContainer;

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
        }
        catch(NullReferenceException)
        {
            //If the properties don't exist yet, create them with default values
            Hashtable initialProperties = new Hashtable();
            initialProperties[currMaxIDKey] = -1;
            initialProperties[lastAddedIndexKey] = -1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(initialProperties);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            AddElement(0);
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

    public void AddElement(int insertAtIndex)
    {
        //get the current MaxID & lastAddedIndex
        Room room = PhotonNetwork.CurrentRoom;
        Hashtable table = room.CustomProperties;
        int currMaxID = (int)table[currMaxIDKey];
        int lastAddedIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties[lastAddedIndexKey];

        //put those old property values in a hashtable, to perform a check-and-set with (See https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state#custom_properties)
        Hashtable expectedProperty = new Hashtable();
        expectedProperty[currMaxIDKey] = currMaxID;
        expectedProperty[lastAddedIndexKey] = lastAddedIndex;

        //Try to set the new property values by check-and-set, this will trigger a callback if it succeeds, which then spawns the new model element.
        //If not nothing happens. This will only fail if two clients spawn an element almost simultaneously
        int newID = currMaxID + 1;
        Hashtable updatedProperty = new Hashtable();
        updatedProperty[currMaxIDKey] = newID;
        updatedProperty[lastAddedIndexKey] = insertAtIndex;
        PhotonNetwork.CurrentRoom.SetCustomProperties(updatedProperty, expectedProperty);
    }

    public void DeleteElement(ClassElementType elementType, int viewID)
    {
        int[] updatedModel;
        switch (elementType)
        {
            case ClassElementType.Field:
                updatedModel = CalculateModelForRemovedElement(fieldsModel, viewID);
                RaiseUpdateClassContentEvent(elementType, updatedModel, fieldsModel);
                break;
            case ClassElementType.Operation:
                updatedModel = CalculateModelForRemovedElement(operationsModel, viewID);
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
        
        Debug.Log($"Properties changed: ID: {propertiesThatChanged[currMaxIDKey]} Index: {propertiesThatChanged[lastAddedIndexKey]}");
        //TODO add new model element at given index with given ID
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

        //bring the element model (IDs & their order) in sync with the updated specification in updatedElements.
        int updatingElementIndex;
        for (updatingElementIndex = 0; updatingElementIndex < updatedElements.Length; updatingElementIndex++)
        {
            if (elementsModel.Count > updatingElementIndex)
            {
                if (updatedElements[updatingElementIndex] != elementsModel[updatingElementIndex])
                {
                    //TODO: This is not right yet
                    if (updatedElements[updatingElementIndex] != elementsModel[updatingElementIndex])//There was an element moved from a different index in the list to the updatingElementIndex or the one at updatigElementIndex was deleted
                    {
                        if(updatedElements.Contains(elementsModel[updatingElementIndex]))//if the ID is still in the updatedElements it was just moved to a different position
                        {
                            int movedElement = elementsModel[updatingElementIndex];
                            elementsModel.RemoveAt(elementsModel[updatingElementIndex]);
                            elementsModel.Insert(updatingElementIndex, movedElement);
                        }
                    }
                    //If an element was deleted it is now step by step being passed to the end of the list
                }
            }
        }

        for (int deleteElementIndex = updatingElementIndex; deleteElementIndex < elementsModel.Count; deleteElementIndex++)
        {
            //delete element at deleteElementIndex, because it was not present in the updatedElements list.
            int elementToDelete = elementsModel[deleteElementIndex];
            //tell Class Side Mirror to delete the respective UI Element (The element with ID == elementToDelete.ViewID)
            classSides.LocalDeleteElement(elementType, elementToDelete);
            //Delete the element from the ContentSynchronizer's internal model
            elementsModel.RemoveAt(deleteElementIndex);
        }
    }

    

    #endregion
}
