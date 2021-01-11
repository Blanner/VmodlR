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
    //disable warnings for private (serialized) fields not being assigned to
    #pragma warning disable 649

    #region Private Serializable Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    
    private GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private GameObject feedbackText;

    [Tooltip("The input field where players enter a new room's name")]
    [SerializeField]
    private InputField newRoomNameInput;

    [Tooltip("Theslider where players set a new room's max player count")]
    [SerializeField]
    private Slider newRoomMaxPlayerSlider;

    #endregion

    #region Private Fields

    private bool isConnecting = false;
    private bool isCreatingRoom = false;

    private string gameVersion = "1";

    private string roomNameToConnectTo;

    #endregion

    #region Monobehaviour Callbacks

    void Awake()
    {
        // makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = this.gameVersion;
    }

    #if UNITY_EDITOR

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            PhotonNetwork.NickName = "UnityEditor";
            CreateRoomInternal("NewRoom");
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            TaskSettings.Instance.ChangeTask(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            TaskSettings.Instance.ChangeTask(0);
        }
    }

    #endif

    #endregion

    #region Public Methods

    private void CreateRoomInternal(string newRoomName)
    {
        Debug.Log("Trying to create room");
        byte maxPlayers = (byte)newRoomMaxPlayerSlider.value;

        feedbackText.SetActive(true);
        controlPanel.SetActive(false);

        roomNameToConnectTo = newRoomName;

        isConnecting = true;
        isCreatingRoom = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(newRoomName, new RoomOptions { MaxPlayers = maxPlayers });
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }

    public void CreateRoom()
    {
        CreateRoomInternal(newRoomNameInput.text);
    }

    public void Connect(string roomName)
    {
        feedbackText.SetActive(true);
        controlPanel.SetActive(false);

        roomNameToConnectTo = roomName;

        isConnecting = true;
        isCreatingRoom = false;

        if(PhotonNetwork.IsConnected)
        {
            Debug.Log("Trying to join room " + roomName);
            PhotonNetwork.JoinRoom(roomName);
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
        Debug.Log("OnConnectedToMaster(). Client is connected to master lobby and ready to join a room.");
        if (isConnecting)
        {
            if(isCreatingRoom)
            {
                PhotonNetwork.CreateRoom(roomNameToConnectTo, new RoomOptions { MaxPlayers = (byte)newRoomMaxPlayerSlider.value });
            }
            else
            {
                Debug.Log("Trying to join a random room...");
                PhotonNetwork.JoinRoom(roomNameToConnectTo);
            }
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available. Creating a new one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        isConnecting = false;
        isCreatingRoom = false;

        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed: " + message);

        isConnecting = false;
        isCreatingRoom = false;

        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room: " + message);

        isConnecting = false;
        isCreatingRoom = false;

        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Server.");
        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        isConnecting = false;
        isCreatingRoom = false;
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

            //Set the active task for this room as a room property
            TaskSettings.Instance.WriteTaskRoomProperty();

            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("MainEnvironment");
        }
    }

    #endregion
}
