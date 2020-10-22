using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class OpenRoomsManager : MonoBehaviourPunCallbacks
{
    public RectTransform contentTransform;

    public Lobby lobbyManager;

    public GameObject roomListEntryPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        UpdateOpenRoomsListUI(roomList);
    }

    private void UpdateOpenRoomsListUI(List<RoomInfo>  roomList)
    {
        //Destroy all children
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            DestroyImmediate(contentTransform.GetChild(i).gameObject);
        }
        
        //Create a new room ui for every entry in the roomList
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.MaxPlayers <= 0)
                continue;

            GameObject newRoomEntry = Instantiate(roomListEntryPrefab, contentTransform);
            newRoomEntry.GetComponent<RoomUI>().Init(roomInfo);
        }
    }
}
