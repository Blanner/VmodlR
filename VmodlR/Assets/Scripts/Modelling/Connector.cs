using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// A connecting line between two classes that possibly has an arrow tip at one end. 
/// It sticks to the attached classes and can be moved by the playerwhen he grabs the grab volumes belonging to this connector.
/// </summary>
[RequireComponent(typeof(ArrowHeadSwitcher))]
public class Connector : MonoBehaviourPun, IOnEventCallback
{
    #region public fields

    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - does not point. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public ClassConnectionHub originClass;
    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - points. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public ClassConnectionHub targetClass;
    /// <summary>
    /// The arrow head that sits on the target end of this connector or null if this connector is not directed
    /// </summary>
    public ArrowHeadSwitcher arrowHeadManager;

    public ConnectorGrabVolume originGrabVolume;
    public ConnectorGrabVolume targetGrabVolume;

    #endregion

    #region private fields

    /// <summary>
    /// The position relative to the origin class where the origin end of the connection should sit
    /// </summary>
    private Vector3 localOriginConnectionPoint;
    /// <summary>
    /// THe position relative to the target class where the target end of the connection should sit
    /// </summary>
    private Vector3 localTargetConnectionPoint;

    #endregion

    #region Monobehaviour Callbacks

    // Start is called before the first frame update
    protected void Start()
    {
        arrowHeadManager = GetComponent<ArrowHeadSwitcher>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void OnDestroy()
    {
        Debug.Log($"\nOnDestory called on GO {gameObject.name}");
        //Detach the connector from all connected classes locally on this client
        LocalDetachFromClass(originGrabVolume);
        LocalDetachFromClass(targetGrabVolume);
        //Detach this connector from all connected classes across the network
        RemoteUpdateAttachState(ConnectorEndType.Origin, originClass, null, localOriginConnectionPoint);
        RemoteUpdateAttachState(ConnectorEndType.Target, targetClass, null, localTargetConnectionPoint);
    }

    #endregion

    #region Public Methods

    public void ChangeConnectorType(ConnectorTypes connectorType)
    {
        arrowHeadManager.ChangeConnectorType(connectorType);
    }

    /// <summary>
    /// Detaches the connector from the class that is currently at the end of the connector the the given ConnectorGrabVolume corresponds to.
    /// </summary>
    /// <param name="connectorEndGrabVolume"></param>
    public void LocalDetachFromClass(ConnectorGrabVolume connectorEndGrabVolume)
    {
        if(originGrabVolume == connectorEndGrabVolume)
        {
            if (originClass != null)
            {
                originClass.RemoveConnector(this);
            }
            originClass = null;
        }
        else if(targetGrabVolume == connectorEndGrabVolume)
        {
            if(targetClass != null)
            {
                targetClass.RemoveConnector(this);
            }
            targetClass = null;
        }
        else
        {
            Debug.LogError("Called DetachFromClass() with a grab volume not belonging to this connector.");
        }
    }

    /// <summary>
    /// Detaches this connector from a class that is being destroyed across the network
    /// Ensures that the cached attach state event is updated acordingly
    /// </summary>
    /// <param name="destroyedClass"></param>
    public void OnDestroyAttachedClass(ClassConnectionHub destroyedClass)
    {
        if(destroyedClass == targetClass)
        {
            if (targetClass != null)
            {
                targetClass.RemoveConnector(this);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //Update the attach state event that is cached on the server
                //we do this only on master client to ensure the event is not unnecessarily updated from all clients
                RemoteUpdateAttachState(ConnectorEndType.Target, targetClass, null, localTargetConnectionPoint);
            }
            targetClass = null;
        }
        else if(destroyedClass == originClass)
        {
            if (originClass != null)
            {
                originClass.RemoveConnector(this);
            }

            if(PhotonNetwork.IsMasterClient)
            {
                RemoteUpdateAttachState(ConnectorEndType.Origin, originClass, null, localOriginConnectionPoint);
            }
            originClass = null;
        }
        else
        {
            Debug.LogError("Called DetachFromClass() for a class not attached to this connector.");
        }
    }

    /// <summary>
    /// Tries to attach the end of the connector that the given ConnectorGrabVolume corresponds to to a class that this end of the connector currently points at. 
    /// If there is no class in that direction the connectors end stays loose.
    /// This updates the Transform of the connector, only call on the client that currently owns the connector
    /// </summary>
    public void CalculateNewAttachement(ConnectorGrabVolume connectorEndGrabVolume)
    {
        Vector3 attachSearchOrigin = originGrabVolume.transform.position + ((targetGrabVolume.transform.position - originGrabVolume.transform.position) / 2);//The mid point between both grab volumes
        Vector3 attachSearchDirection;

        if (originGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward * -1.0f;
            //check if we can attach to a class
            UpdateAttachState(ConnectorEndType.Origin, attachSearchOrigin, attachSearchDirection);
            UpdateTransform();
        }
        else if (targetGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward;
            //check if we can attach to a class
            UpdateAttachState(ConnectorEndType.Target, attachSearchOrigin, attachSearchDirection);
            UpdateTransform();
        }
        else
        {
            Debug.LogError("Called attachFromClass() with a grab volume not belonging to this connector.");
            return;
        }
    }

    /// <summary>
    /// Recalculates the attachement between connector and class for this connector's end that is attached to the given class
    /// </summary>
    /// <param name="endType"></param>
    public void CalculateNewAttachement(ClassConnectionHub connectedClass)
    {
        if(connectedClass == originClass)
        {
            CalculateNewAttachement(originGrabVolume);
        }
        else if(connectedClass == targetClass)
        {
            CalculateNewAttachement(targetGrabVolume);
        }
    }

    /// <summary>
    /// Callback triggered by attach events on a grab volume. 
    /// This updates the local attach states without affecting the transform, since the transform is already synchronized via PhotonViews
    /// </summary>
    /// <param name="connectorEndGrabVolume"></param>
    public void OnOwnerAttachedToClass(ConnectorGrabVolume connectorEndGrabVolume)
    {
        Vector3 attachSearchOrigin = connectorEndGrabVolume.transform.position;
        Vector3 attachSearchDirection;

        if (originGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward * -1.0f;
            //check if we can attach to a class
            UpdateAttachState(ConnectorEndType.Origin, attachSearchOrigin, attachSearchDirection);
        }
        else if (targetGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward;
            //check if we can attach to a class
            UpdateAttachState(ConnectorEndType.Target, attachSearchOrigin, attachSearchDirection);
        }
        else
        {
            Debug.LogError("Called attachFromClass() with a grab volume not belonging to this connector.");
            return;
        }
    }

    /// <summary>
    /// Tries to update the attach state of this connector at the given end to a class that this end of the connector currently points at. 
    /// If there is no class in that direction the connectors end stays loose or gets detatched from its current class.
    /// The attach state is the synchronized across all clients via a raised event
    /// This does not update the Transform of the connector, only the attach states.
    /// </summary>
    public void UpdateAttachState(ConnectorEndType connectorEnd, Vector3 attachSearchOrigin, Vector3 attachSearchDirection)
    {
        Vector3 oldLocalConnectionPointToClass = (connectorEnd == ConnectorEndType.Origin) ? localOriginConnectionPoint : localTargetConnectionPoint;

        ClassConnectionHub oldAttachedClass = (connectorEnd == ConnectorEndType.Origin) ? originClass : targetClass;
        ClassConnectionHub newAttachedClass = calculateNewConnectionToClass(attachSearchOrigin, attachSearchDirection, out Vector3 newConnectionPointWorld);

        if (newAttachedClass != null)
        {
            AttachToClass(newAttachedClass, connectorEnd);
            UpdateConnectionPointToClass(newConnectionPointWorld, connectorEnd);
        }

        RemoteUpdateAttachState(connectorEnd, oldAttachedClass, newAttachedClass, oldLocalConnectionPointToClass);
    }
    /// <summary>
    /// Updates the transform values of the connector and its related objects if the respective end of the connector is connected to a class.
    /// If it is not connected, it follows the position of the respective grab Volume.
    /// Position, rotation and scale are updated as needed for the connector line, its arrow head (if it has one) and the grab volumes (if they are not currently grabbed)
    /// </summary>
    public void UpdateTransform()
    {
        photonView.RequestOwnership();

        Vector3 newOriginPos;
        Vector3 newTargetPos;

        
        //calculate origin and target position of the moved connector acording to the new positions of its classes
        //This is done by transforming the target/origin positions local to the respective class back to global positions based on the new transform values of the classes
        if (originClass != null)
        {
            newOriginPos = originClass.transform.TransformPoint(localOriginConnectionPoint);
        }
        else
        {
            newOriginPos = originGrabVolume.transform.position;
        }

        if (targetClass != null)
        {
            newTargetPos = targetClass.transform.TransformPoint(localTargetConnectionPoint);
        }
        else
        {
            newTargetPos = targetGrabVolume.transform.position;
        }

        //calculate the vector that describes the orientation and length of the new connector
        Vector3 newConnectorLineSegment = newTargetPos - newOriginPos; 

        //move the connector to the new origin
        transform.position = newOriginPos;
        //scale the connector to the correct new length
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newConnectorLineSegment.magnitude - arrowHeadManager.GetTipDistance());
        //update the orintation of the connector
        transform.rotation = Quaternion.LookRotation(newConnectorLineSegment, Vector3.up);
        //update the position of this connectors arrow head if it has one.
        if(arrowHeadManager.activeArrowHead != null)
        {
            arrowHeadManager.activeArrowHead.UpdateTransform(newTargetPos, newConnectorLineSegment);
        }
        originGrabVolume.UpdateTransform(newOriginPos);
        targetGrabVolume.UpdateTransform(newTargetPos);
    }



    #endregion

    #region private Methods

    /// <summary>
    /// Resets the attaching position (local to the attached class) of the connectors given end to the given world position
    /// If the origin end is currently not attached to a class nothing happens.
    /// </summary>
    private void UpdateConnectionPointToClass(Vector3 connectionPointWorldPos, ConnectorEndType connectorEnd)
    {
        switch(connectorEnd)
        {
            case ConnectorEndType.Origin:
                if (originClass != null)
                {
                    localOriginConnectionPoint = originClass.transform.InverseTransformPoint(connectionPointWorldPos);
                }
                break;
            case ConnectorEndType.Target:
                if (targetClass != null)
                {
                    localTargetConnectionPoint = targetClass.transform.InverseTransformPoint(connectionPointWorldPos);
                }
                break;
        }
    }

    /// <summary>
    /// Checks if there is a class in the given search direction that is less than Properties.connectorAttachToClassDistance meters away when measured from the given searchOrigin
    /// If so this class is returned and 
    /// </summary>
    /// <param name="newConnectionPointWorld">The world space position at which the search ray consting of the searchorigin and searchDirection hit the found class or Vector.zero if no class was found.</param>
    /// <returns>The found class if there is one or null if no class was found</returns>
    private ClassConnectionHub calculateNewConnectionToClass(Vector3 searchOrigin, Vector3 searchDirection, out Vector3 newConnectionPointWorld)
    {
        int classLayerMask = 1 << LayerMask.NameToLayer("Class");
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(searchOrigin, searchDirection), out hitInfo, Properties.connectorAttachToClassDistance, classLayerMask))
        {
            Debug.Log($"Raycast hit {hitInfo.transform.name}");
            ClassConnectionHub hitClass = hitInfo.transform.GetComponent<ClassConnectionHub>();
            if (hitClass != null)
            {
                newConnectionPointWorld = hitInfo.point;
                return hitClass;
            }
        }

        //No class was hit
        newConnectionPointWorld = Vector3.zero;
        return null;
    }

    private void AttachToClass(ClassConnectionHub attachClass, ConnectorEndType connectorEndType)
    {
        //In case this end is still attached to another class, we first detach it
        DetachFromClass(connectorEndType);

        switch(connectorEndType)
        {
            case ConnectorEndType.Origin:
                attachClass.AddConnector(this);
                originClass = attachClass;
                break;
            case ConnectorEndType.Target:
                attachClass.AddConnector(this);
                targetClass = attachClass;
                break;
        }
    }

    private void DetachFromClass(ConnectorEndType connectorEndType)
    {
        switch (connectorEndType)
        {
            case ConnectorEndType.Origin:
                if(originClass != null)
                {
                    originClass.RemoveConnector(this);
                    originClass = null;
                }
                break;
            case ConnectorEndType.Target:
                if(targetClass != null)
                {
                    targetClass.RemoveConnector(this);
                    targetClass = null;
                }
                break;
        }
    }

    #endregion

    #region Network Implementation



    /// <summary>
    /// Updates the attach state on all other instances of this connector and the respective classes across the network by raising an event and deleting the event it replaces.
    /// </summary>
    /// <param name="connectorEndType">Wether to update the attach state to the target or origin class</param>
    /// <param name="previousAttachedClass">the class that was attached to the connector end before the change</param>
    /// <param name="newAttachedClass">the class that is now (after the change) attached to the connector end</param>
    /// <param name="oldLocalConnectionPoint">the local connection point the connector end had to the <paramref name="previousAttachedClass"/></param>
    public void RemoteUpdateAttachState(ConnectorEndType connectorEndType, ClassConnectionHub previousAttachedClass, ClassConnectionHub newAttachedClass, Vector3 oldLocalConnectionPoint)
    {
        Debug.Log($"\nUpdating Attach State on {gameObject.name}");

        byte eventCode = (connectorEndType == ConnectorEndType.Target) ? EventCodes.updateTargetEndAttachState : EventCodes.updateOriginEndAttachState;
        Vector3 newLocalConnectionPos = (connectorEndType == ConnectorEndType.Target) ? localTargetConnectionPoint : localOriginConnectionPoint;

        //recreate the content of the last attach event
        Hashtable oldContent = new Hashtable();
        oldContent.Add("ConnectorViewID", this.photonView.ViewID);
        oldContent.Add("ClassViewID", (previousAttachedClass == null) ? -1 : previousAttachedClass.photonView.ViewID);
        oldContent.Add("LocalConnectionPos", oldLocalConnectionPoint); 
        //Delete the preceding attach event that will be overwriten by a new one afterwards, this is necessary so the rooms event cache does not fill up
        //delete the last change event from the room's event cache by filtering by its content
        RaiseEventOptions deleteEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        if (!PhotonNetwork.RaiseEvent(eventCode, oldContent, deleteEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("Event was not deleted!");
        }

        //create the new content for the new event
        Hashtable newContent = new Hashtable();
        newContent.Add("ConnectorViewID", photonView.ViewID);
        newContent.Add("ClassViewID", (newAttachedClass == null) ? -1 : newAttachedClass.photonView.ViewID);
        newContent.Add("LocalConnectionPos", newLocalConnectionPos);

        //Debug.Log($"\nRaising Event on {gameObject.name}");

        //Raise an Attach Event so all instances of this grab volume on other clients can instruct their connector to update his attach state
        RaiseEventOptions createEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        if (!PhotonNetwork.RaiseEvent(eventCode, newContent, createEventOptions, SendOptions.SendReliable))
        {
            Debug.LogError("\nCould not raise event");
        }

        //Debug.Log($"\nRaised Event on {gameObject.name}");
    }

    public void OnEvent(EventData photonEvent)
    {
        if (EventCodes.IsUpdateOriginAttachmentEvent(photonEvent.Code) || EventCodes.IsUpdateTargetAttachmentEvent(photonEvent.Code))
        {
            //Debug.Log($"\n update target end attach state of Connector Event on {this.gameObject.name}");
            //extract the sent data from the event
            Hashtable eventData = (Hashtable)photonEvent.CustomData;
            int connectorViewID = (int)eventData["ConnectorViewID"];
            if (connectorViewID == photonView.ViewID)
            {
                //Debug.Log($"\nEvent triggers attaching to class on {this.gameObject.name}");
                //Attach/detatch
                int classViewID = (int)eventData["ClassViewID"];
                Vector3 localConnectionPos = (Vector3)eventData["LocalConnectionPos"];

                if(classViewID == -1)//Detaching
                {
                    if (EventCodes.IsUpdateOriginAttachmentEvent(photonEvent.Code))
                    {
                        DetachFromClass(ConnectorEndType.Origin);
                        localOriginConnectionPoint = localConnectionPos;
                    }
                    else //EventCodes.IsUpdateTargetAttachmentEvent(photonEvent.Code) is true
                    {
                        DetachFromClass(ConnectorEndType.Target);
                        localTargetConnectionPoint = localConnectionPos;
                    }
                }
                else//Attaching
                {
                    PhotonView classView = PhotonView.Find(classViewID);
                    if (classView == null)
                    {
                        Debug.LogError($"Photon View with ID {classViewID} not found!");
                        return;
                    }

                    ClassConnectionHub attachClass = classView.transform.GetComponent<ClassConnectionHub>();
                    if (attachClass == null)
                    {
                        Debug.LogError($"UMLClass instance not found on PhotonView {classViewID}");
                        return;
                    }

                    if (EventCodes.IsUpdateOriginAttachmentEvent(photonEvent.Code))
                    {
                        AttachToClass(attachClass, ConnectorEndType.Origin);
                        localOriginConnectionPoint = localConnectionPos;
                    }
                    else //EventCodes.IsUpdateTargetAttachmentEvent(photonEvent.Code) is true
                    {
                        AttachToClass(attachClass, ConnectorEndType.Target);
                        localTargetConnectionPoint = localConnectionPos;
                    }
                    
                }
            }
        }
     
    }

    #endregion
}
