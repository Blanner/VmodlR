using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class DeletableElement : MonoBehaviourPun
{
    public Renderer deletableRenderer;

    private void Start()
    {
        if (deletableRenderer == null)
        {
            Debug.LogError($"Deletable Renderer of DeletableElement on {gameObject.name} is not set!");
            this.enabled = false;
        }
    }

    /// <summary>
    /// Deletes a specific room object across the entwork. This is ensured to be only done on the masterclient 
    /// by either doing it localy if this is the master client or calling this method as an RPC on the master client otherwise. 
    /// </summary>
    [PunRPC]
    public void DeleteOnMaster(int photonViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(PhotonView.Find(photonViewID));
        }
        else
        {
            //Call this function as an RPC on the master client, if this is not the master
            photonView.RPC("DeleteOnMaster", RpcTarget.MasterClient, photonViewID);
        }
    }
}
