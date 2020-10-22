using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Text))]
public class PlayerCountUpdater : MonoBehaviourPunCallbacks
{

    private Text playerCountText;

    // Start is called before the first frame update
    void Start()
    {
        playerCountText = GetComponent<Text>();
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
}
