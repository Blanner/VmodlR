using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

//This is derived from the Photon Documentation at https://doc.photonengine.com/en-us/pun/v2/gameplay/rpcsandraiseevent
//Specifically the sections concerning RaiseEvent & Events in general
/// <summary>
/// Synchronizes a class name across the network and across class sides.
/// Place this on the input field it is supposed to synchronize and assign the class' ClassSideMirror & PhotonView
/// </summary>
[RequireComponent(typeof(InputField))]
public class ClassNameSynchronizer : MonoBehaviour, IOnEventCallback
{
    #region Public Fields
    
    public ClassSideMirror classSides;
    public PhotonView classPhotonView;

    #endregion

    #region Private Fields

    /// <summary>
    /// This is a model of the class name, we need this so we still have the previous value accessible when we are notified that input fields text was changed
    /// </summary>
    private string className = "";
    private InputField synchronizeInputField;


    #endregion

    #region Monobehaviour Callbacks

    void Start()
    {
        synchronizeInputField = GetComponent<InputField>();
        if (synchronizeInputField == null)
        {
            Debug.LogError("Input Field of ClassNameSynchronizer not assigned! No synchronization will be possible!");
            this.enabled = false;
        }
        else
        {
            className = synchronizeInputField.text;
        }

        if(classSides == null)
        {
            Debug.LogError("Class Side Mirror of ClassNameSynchronizer not assigned! No synchronization will be possible!");
            this.enabled = false;
        }

        if (classPhotonView == null)
        {
            Debug.LogError("Class PhotonView of ClassNameSynchronizer not assigned! No synchronization will be possible!");
            this.enabled = false;
        }
    }

    //Without adding this class as a Callback target, photon will not call the OnEvent Callback.
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sends an event to all clients (including itself) to change the input field value and update the className model (string variable)
    /// The actual change of the field happens in the event callback for all client synchronously
    /// This should only be called on the client where a user made a change to the className field. 
    /// </summary>
    /// <param name="newName"></param>
    public void RemoteChangeName(string newName)
    {
        //recreate the content of the last change event received
        Hashtable oldContent = new Hashtable();
        oldContent.Add("PhotonViewID", classPhotonView.ViewID);
        oldContent.Add("NewClassName", className);//this is still the "old" class name

        //delete the last change event from the room's event cache by filtering by its content
        RaiseEventOptions deleteEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        if (!PhotonNetwork.RaiseEvent(EventCodes.synchronizeClassName, oldContent, deleteEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not deleted!");
        }

        //create the new content for the new event
        Hashtable newConent = new Hashtable();
        newConent.Add("PhotonViewID", classPhotonView.ViewID);
        newConent.Add("NewClassName", newName);

        //raise the new change event, that replaces the one we just deleted
        RaiseEventOptions createEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.synchronizeClassName, newConent, createEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// Changes the text of the synchronized input field to the new value without triggering a new synchronization.
    /// Call this to apply a change that was received as an event over the network to this local name field
    /// </summary>
    /// <param name="newName"></param>
    public void LocalChangeName(string newName)
    {
        className = newName;
        synchronizeInputField.SetTextWithoutNotify(className);
    }

    #region IOnEventCallback Implementation

    //This is called by photon for any raised event (can be filtered by interest groups, haven't read up about that yet)
    //So the first check, if the event code is the one we expect is cruitial, because the function is also called for some builtin events used by photon internally (EventCodes 200..255)
    public void OnEvent(EventData photonEvent)
    {
        //Check if the event is a synchronization event for a class
        if (photonEvent.Code == EventCodes.synchronizeClassName)
        {
            //extract the sent data from the event
            Hashtable eventData = (Hashtable)photonEvent.CustomData;
            int synchronizeClassViewID = (int)eventData["PhotonViewID"];
            //check if the class that the event wants to synchronize is this class
            if (synchronizeClassViewID == classPhotonView.ViewID)
            {
                string newClassName = (string)eventData["NewClassName"];
                classSides.LocalChangeClassName(newClassName);
            }
        }
    }

    #endregion

    #endregion
}