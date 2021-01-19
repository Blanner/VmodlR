using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ArrowHead : MonoBehaviourPun
{
    [Tooltip("The distance between the tip of the arrow and the point where the connector this arrow head sits on should end")]
    public float TipDistance;

    public ConnectorTypes ConnectorType;

    /// <summary>
    /// Repositions and rotates the Arrow according to the given Position and forward Direction.
    /// </summary>
    /// <param name="newPosition"></param>
    /// <param name="forwardDirection"></param>
    public void UpdateTransform(Vector3 newPosition, Vector3 forwardDirection)
    {
        photonView.RequestOwnership();
        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
    }
}
