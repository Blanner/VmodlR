using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A connecting line between two classes that possibly has an arrow tip at one end. 
/// It sticks to the attached classes and can be moved by the playerwhen he grabs the grab volumes belonging to this connector.
/// </summary>
public class Connector : NetworkModelElement
{
    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - does not point. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public Class originClass;
    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - points. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public Class targetClass;
    /// <summary>
    /// The arrow head that sits on the target end of this connector or null if this connector is not directed
    /// </summary>
    public ArrowHead arrowHead;

    public ConnectorGrabVolume originGrabVolume;
    public ConnectorGrabVolume targetGrabVolume;

    private Vector3 ConnectorMidPosition { get { return transform.GetChild(0).position; } }

    /// <summary>
    /// The position relative to the origin class where the origin end of the connection should sit
    /// </summary>
    private Vector3 originLocalPosition;
    /// <summary>
    /// THe position relative to the target class where the target end of the connection should sit
    /// </summary>
    private Vector3 targetLocalPosition;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Detaches the connector from the class that is currently at the end of the connector the the given ConnectorGrabVolume corresponds to.
    /// </summary>
    /// <param name="connectorEndGrabVolume"></param>
    public void DetachFromClass(ConnectorGrabVolume connectorEndGrabVolume)
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
    /// Tries to attach the end of the connector that the given ConnectorGrabVolume corresponds to to a class that this end of the connector currently points at. 
    /// If there is no class in that direction the connectors end stays loose.
    /// </summary>
    public void AttachToClass(ConnectorGrabVolume connectorEndGrabVolume)
    {
        Vector3 attachSearchOrigin = connectorEndGrabVolume.transform.position;
        Vector3 attachSearchDirection;

        if (originGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward * -1.0f;
            //check if we can attach to a class
            Vector3 newConnectionPointWorld;
            originClass = calculateNewConnectionToClass(attachSearchOrigin, attachSearchDirection, out newConnectionPointWorld);
            if(originClass != null)
            {
                originClass.AddConnector(this);
                updateConnectionPointToOriginClass(newConnectionPointWorld);
            }
            
            UpdateTransformFromClassConnections();
        }
        else if (targetGrabVolume == connectorEndGrabVolume)
        {
            attachSearchDirection = transform.forward;
            //check if we can attach to a class
            Vector3 newConnectionPointWorld;
            targetClass = calculateNewConnectionToClass(attachSearchOrigin, attachSearchDirection, out newConnectionPointWorld);
            if (targetClass != null)
            {
                targetClass.AddConnector(this);
                updateConnectionPointToTargetClass(newConnectionPointWorld);
            }
            UpdateTransformFromClassConnections();
        }
        else
        {
            Debug.LogError("Called attachFromClass() with a grab volume not belonging to this connector.");
            return;
        }
    }

    /// <summary>
    /// Updates the transform values of the connector and its related objects if the respective end of the connector is connected to a class.
    /// position, rotation and scale are updated as needed for the connector line, its arrow head (if it has one) and the grab volumes (if they are not currently grabbed)
    /// </summary>
    public void UpdateTransformFromClassConnections()
    {
        RequestOwnership();

        Vector3 newOriginPos;
        Vector3 newTargetPos;

        
        //calculate origin and target position of the moved connector acording to the new positions of its classes
        //This is done by transforming the target/origin positions local to the respective class back to global positions based on the new transform values of the classes
        if (originClass != null)
        {
            newOriginPos = originClass.transform.TransformPoint(originLocalPosition);
        }
        else
        {
            newOriginPos = originGrabVolume.transform.position;
        }

        if (targetClass != null)
        {
            newTargetPos = targetClass.transform.TransformPoint(targetLocalPosition);
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
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newConnectorLineSegment.magnitude - arrowHead.TipDistance);
        //update the orintation of the connector
        transform.rotation = Quaternion.LookRotation(newConnectorLineSegment, Vector3.up);//TODO: is Vector3.up correct here?
        //update the position of the this connectors arrow head if it has one.
        if(arrowHead != null)
        {
            arrowHead.UpdateTransform(newTargetPos, newConnectorLineSegment);
        }
        originGrabVolume.UpdateTransform(newOriginPos);
        targetGrabVolume.UpdateTransform(newTargetPos);
    }


    /// <summary>
    /// Resets the attaching position (local to the attached class) of the connectors target end to the given world position
    /// If the target end is currently not attached to a class nothing happens.
    /// </summary>
    /// <param name="connectionPointWorldPos"></param>
    private void updateConnectionPointToTargetClass(Vector3 connectionPointWorldPos)
    {
        if(targetClass != null)
        {
            targetLocalPosition = targetClass.transform.InverseTransformPoint(connectionPointWorldPos);
        }
    }

    /// <summary>
    /// Resets the attaching position (local to the attached class) of the connectors origin end to the given world position
    /// If the origin end is currently not attached to a class nothing happens.
    /// </summary>
    private void updateConnectionPointToOriginClass(Vector3 connectionPointWorldPos)
    {
        if (originClass != null)
        {
            originLocalPosition = originClass.transform.InverseTransformPoint(connectionPointWorldPos);
        }
    }

    /// <summary>
    /// Checks if there is a class in the given search direction that is less than Properties.connectorAttachToClassDistance meters away when measured from the given searchOrigin
    /// If so this class is returned and 
    /// </summary>
    /// <param name="newConnectionPointWorld">The world space position at which the search ray consting of the searchorigin and searchDirection hit the found class or Vector.zero if no class was found.</param>
    /// <returns>The found class if there is one or null if no class was found</returns>
    private Class calculateNewConnectionToClass(Vector3 searchOrigin, Vector3 searchDirection, out Vector3 newConnectionPointWorld)
    {
        int classLayerMask = 1 << LayerMask.NameToLayer("Class");
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(searchOrigin, searchDirection), out hitInfo, Properties.connectorAttachToClassDistance, classLayerMask))
        {
            Class hitClass = hitInfo.transform.GetComponent<Class>();
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
}
