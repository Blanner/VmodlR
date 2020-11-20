using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(IGrabListener))]
public class OVRNetworkGrabbable : OVRGrabbable
{
    private PhotonView photonView;
    private IGrabListener grabListener;

    protected new void Start()
    {
        base.Start();
        photonView = GetComponent<PhotonView>();
        grabListener = GetComponent<IGrabListener>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        //Debug.Log($"NetworkGrabbable {this.gameObject.name} beginning grab: Calling Request Ownership.");
        photonView.RequestOwnership();
        base.GrabBegin(hand, grabPoint);
        grabListener.OnGrabBegin();
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        grabListener.OnGrabEnd();
    }
}
