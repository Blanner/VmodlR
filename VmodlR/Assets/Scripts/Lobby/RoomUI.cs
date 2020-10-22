using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

public class RoomUI : MonoBehaviour
{
    public Text roomNameText;
    public Text playerNumberText; 

    public void Init(RoomInfo roomInfo)
    {
        roomNameText.text = roomInfo.Name;
        playerNumberText.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }

    public void JoinRoom()
    {
        Lobby lobbyManager = GameObject.FindObjectOfType<Lobby>();
        lobbyManager.Connect(roomNameText.text);
    }
}
