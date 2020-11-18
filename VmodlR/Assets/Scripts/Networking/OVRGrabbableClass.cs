using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class OVRGrabbableClass : OVRGrabbable
{
    private Class classElement;

    private new void Start()
    {
        base.Start();
        classElement = GetComponent<Class>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        classElement.BeginMovement();
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        classElement.EndMovement();
    }
}
