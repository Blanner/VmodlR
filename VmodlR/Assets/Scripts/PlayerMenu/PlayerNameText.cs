using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

[RequireComponent(typeof(Text))]
public class PlayerNameText : MonoBehaviour
{
    public PhotonView playerPhotonView;

    private Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText = GetComponent<Text>();
        nameText.text = playerPhotonView.Owner.NickName;
        enabled = false;
    }
}
