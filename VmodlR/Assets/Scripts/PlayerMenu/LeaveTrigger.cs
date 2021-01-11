using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class LeaveTrigger : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LeaveGame();
        }
    }
#endif

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadSceneAsync("Lobby");
    }
}
