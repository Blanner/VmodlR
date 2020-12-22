using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    public RectTransform contentTransform;

    public GameObject PlayerListEntryPrefab;

    public void Start()
    {
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnLeftRoom");
        base.OnJoinedRoom();
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnLeftRoom");
        base.OnJoinedRoom();
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;

        //Destroy all children
        int childCount = contentTransform.childCount;//this will be changing during the loop, so we have to cache it.
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(contentTransform.GetChild(0).gameObject);
        }

        //Create a new player entry for every player in the player list
        foreach (Player player in players)
        {            
            GameObject newPlayerEntry = Instantiate(PlayerListEntryPrefab, contentTransform);
            newPlayerEntry.GetComponent<Text>().text = player.NickName;
        }
    }
}