using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// This class contains the model of a class' name and synchronized it across the network and across all class sides.
/// </summary>
public class ClassElementSynchronizer : MonoBehaviour, IOnEventCallback
{
    public ClassSideMirror classSideMirror;

    public ClassSideElement syncElement;

    private string elementValue;

    public void Initialize(ClassSideMirror classSideMirror, ClassSideElement syncElement)
    {
        this.classSideMirror = classSideMirror;
        this.syncElement = syncElement;
        elementValue = syncElement.Value;
    }

    void Start()
    {
        if(syncElement != null)
        {
            elementValue = syncElement.Value;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void RemoteChangeValue(string newValue)
    {
        //photonView.RequestOwnership();

        //recreate the content of the last change event received
        Hashtable oldContent = new Hashtable();
        oldContent.Add("ElementID", syncElement.ElementID);
        oldContent.Add("NewElementText", elementValue);//this is still the "old" class name
        oldContent.Add("ElementType", syncElement.ElementType);

        //delete the last change event from the room's event cache by filtering by its content
        RaiseEventOptions deleteEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        if(!PhotonNetwork.RaiseEvent(EventCodes.synchronizeClassElement, oldContent, deleteEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not deleted!");
        }

        //create the new content for the new event
        Hashtable newConent = new Hashtable();
        newConent.Add("ElementID", syncElement.ElementID);
        newConent.Add("NewElementText", newValue);
        newConent.Add("ElementType", syncElement.ElementType);

        //raise the new change event, that replaces the one we just deleted
        RaiseEventOptions createEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        if (!PhotonNetwork.RaiseEvent(EventCodes.synchronizeClassElement, newConent, createEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not raised!");
        }
    }

    public void LocalChangeValue(string newValue)
    {
        this.elementValue = newValue;
    }

    public void OnEvent(EventData photonEvent)
    {
        //Check if the event is a synchronization event for a class
        if(photonEvent.Code == EventCodes.synchronizeClassElement)
        {
            //extract the sent data from the event
            Hashtable eventData = (Hashtable)photonEvent.CustomData;
            int elementID = (int)eventData["ElementID"];
            //check if the class that the event wants to synchronize is this class
            if(syncElement.ElementID == elementID)
            {
                string newElementValue = (string)eventData["NewElementText"];
                ClassElementType elementType = (ClassElementType)eventData["ElementType"];
                classSideMirror.LocalChangeElement(elementType, elementID, newElementValue);
            }
        }
    }
}
