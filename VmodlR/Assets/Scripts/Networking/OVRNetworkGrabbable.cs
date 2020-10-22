using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class OVRNetworkGrabbable : OVRGrabbable
{
    private PhotonView photonView;

    private new void Start()
    {
        base.Start();
        photonView = GetComponent<PhotonView>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        photonView.RequestOwnership();
        base.GrabBegin(hand, grabPoint);
    }
}
