using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class SideMirror : MonoBehaviourPun, IPunObservable
{
    public List<ClassSide> sides;

    public string className = "ClassName";
    public bool syncLocalChange = false;

    public void OnChangedName(string newName)
    {
        photonView.RequestOwnership();
        localChangeName(newName);
        syncLocalChange = true;
    }

    private void localChangeName(string newName)
    {
        className = newName;
        foreach (ClassSide side in sides)
        {
            side.ChangeName(newName);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Only write if we requested and got ownership (we request when OnChangedName is called)
        if(stream.IsWriting)
        {
            stream.SendNext(className);
        }
        else
        {
            object receivedMessage = stream.ReceiveNext();
            if ( receivedMessage != null)
            {
                localChangeName((string)receivedMessage);
            }
        }
    }

}
