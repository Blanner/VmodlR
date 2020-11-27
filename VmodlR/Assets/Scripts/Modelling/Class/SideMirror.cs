using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SideMirror : MonoBehaviourPun, IOnEventCallback
{
    public const byte synchronizeClassEventCode = 1;

    public List<ClassSide> sides;

    public string className = "Class - Name";

    void Start()
    {
        localChangeName(className);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnChangedName(string newName)
    {
        photonView.RequestOwnership();

        //recreate the content of the last change event received
        Hashtable oldContent = new Hashtable();
        oldContent.Add("PhotonViewID", photonView.ViewID);
        oldContent.Add("NewClassName", className);//this is still the "old" class name

        //delete the last change event from the room's event cache by filtering by its content
        RaiseEventOptions deleteEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        if(!PhotonNetwork.RaiseEvent(synchronizeClassEventCode, oldContent, deleteEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not deleted!");
        }

        //recreate the content of the last change event received
        Hashtable newConent = new Hashtable();
        newConent.Add("PhotonViewID", photonView.ViewID);
        newConent.Add("NewClassName", newName);

        //raise the new change event, that replaces the one we just deleted
        RaiseEventOptions createEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(synchronizeClassEventCode, newConent, createEventOptions, SendOptions.SendReliable);
    }

    private void localChangeName(string newName)
    {
        className = newName;
        foreach (ClassSide side in sides)
        {
            side.ChangeName(newName);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        //Check if the event is a synchronization event for a class
        if(photonEvent.Code == synchronizeClassEventCode)
        {
            //extract the sent data from the event
            Hashtable eventData = (Hashtable)photonEvent.CustomData;
            int synchronizeClassViewID = (int)eventData["PhotonViewID"];
            //check if the class that the event wants to synchronize is this class
            if(synchronizeClassViewID == photonView.ViewID)
            {
                string newClassName = (string)eventData["NewClassName"];
                localChangeName(newClassName);
            }
        }
    }
}
