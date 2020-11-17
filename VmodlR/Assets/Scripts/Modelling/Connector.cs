using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
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

    private Vector3 ConnectorMidPosition { get { return transform.GetChild(0).position; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePosition()
    {
        //calculate origin and target position of the moved connector acording to the new positions of its classes
        Vector3 newOriginPos = calculateNewConnectionToClass(originClass);
        Vector3 newTargetPos = calculateNewConnectionToClass(targetClass);
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
            arrowHead.UpdatePosition(newTargetPos);
        }
    }

    private Vector3 calculateNewConnectionToClass(Class connectingClass)
    {
        int classLayerMask = 1 << LayerMask.NameToLayer("Class");
        foreach (RaycastHit hit in Physics.RaycastAll(new Ray(ConnectorMidPosition, (connectingClass.transform.position - transform.position)), 1000, classLayerMask))
        {
            Class hitClass = hit.transform.GetComponent<Class>();
            if (hitClass != null)
            {
                if (hitClass == connectingClass)
                {
                    return hit.point;
                }
            }
        }

        Debug.LogError($"Could not find new connection point for Class {connectingClass.gameObject.name} on connector {gameObject.name}");
        return Vector3.zero;
    }
}
