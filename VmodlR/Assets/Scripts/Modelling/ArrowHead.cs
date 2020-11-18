using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHead : NetworkModelElement
{
    [Tooltip("The distance between the tip of the arrow and the point where the connector this arrow head sits on should end")]
    public float TipDistance;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    public void UpdateTransform(Vector3 newPosition, Vector3 forwardDirection)
    {
        RequestOwnership();
        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
    }
}
