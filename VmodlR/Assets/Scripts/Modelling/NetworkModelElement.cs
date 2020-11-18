using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

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
