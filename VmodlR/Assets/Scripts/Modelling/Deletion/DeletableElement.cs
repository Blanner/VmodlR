using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class DeletableElement : MonoBehaviour
{
    public PhotonView rootPhotonView;
    public Renderer deletableRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (rootPhotonView == null)
        {
            Debug.LogError($"Root Photon View of DeletableElement on {gameObject.name} is not set!");
            this.enabled = false;
        }
        if(deletableRenderer == null)
        {
            Debug.LogError($"Deletable Renderer of DeletableElement on {gameObject.name} is not set!");
            this.enabled = false;
        }
    }
}
