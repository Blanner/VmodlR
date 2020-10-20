// --------------------------------------------------------------------------------------------------------------------
// File based on following copyright:
//
//<copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

public class Lobby : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private GameObject feedbackText;

    [Tooltip("The maximum number of players per room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 5;

    #endregion

    #region Private Fields

    private bool isConnecting = false;

    private string gameVersion = "1";

    #endregion

    #region Monobehaviour Callbacks

    void Awake()
    {
        // makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        feedbackText.SetActive(false);
        controlPanel.SetActive(true);
    }

    #if UNITY_EDITOR

    void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            PhotonNetwork.NickName = "UnityEditor";
            Connect();
        }
    }

    #endif

    #endregion

    #region Public Methods

    public void Connect()
    {
        feedbackText.SetActive(true);
        controlPanel.SetActive(false);

        isConnecting = true;

        if(PhotonNetwork.IsConnected)
        {
            Debug.Log("Trying to join a random room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }

    #endregion

    #region Monobehaviour PUN Callbacks

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            Debug.Log("OnConnectedToMaster(). Client is connected to master lobby and ready to join a room.");
            Debug.Log("Trying to join a random room...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available. Creating a new one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Server.");
        feedbackText.SetActive(false);
        controlPanel.SetActive(true);

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        isConnecting = false;
    }

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    public override void OnJoinedRoom()
    {
        Debug.Log("Client joined a room as Player " + PhotonNetwork.CurrentRoom.PlayerCount);

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Loading MainEnvironment");

            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("MainEnvironment");
        }
    }

    #endregion
}
