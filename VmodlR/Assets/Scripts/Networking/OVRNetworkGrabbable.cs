using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OVRNetworkGrabbable : OVRGrabbable
{
    private PhotonView photonView;

    protected new void Start()
    {
        base.Start();
        photonView = GetComponent<PhotonView>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        //Debug.Log($"NetworkGrabbable {this.gameObject.name} beginning grab: Calling Request Ownership.");
        photonView.RequestOwnership();
        base.GrabBegin(hand, grabPoint);
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
    }
}
