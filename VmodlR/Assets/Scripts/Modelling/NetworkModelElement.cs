using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

/// <summary>
/// provides the Network calls IsMine and RequestOwnership of the attached Photon view to classes inherinting from this one.
/// Use this as a base class of all model elements that are not NetworkGrabbles but still need to Request Ownership when moved.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkModelElement : MonoBehaviour
{
    private PhotonView photonView;

    // Start is called before the first frame update
    protected void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected void RequestOwnership()
    {
        photonView.RequestOwnership();
    }

    protected bool IsMine()
    {
        return photonView.IsMine;
    }
}
