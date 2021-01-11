using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class TaskSettings : MonoBehaviour
{
    public const string taskKey = "IsTaskAActvie";
    public bool isTaskAActive = true;

    public static TaskSettings Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Too many TaskSettings instances in one scene. There must always be exactly one instance in a scene.", this);
            this.enabled = false;
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(this);
        
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void WriteTaskRoomProperty()
    {
        Hashtable initialProperties = new Hashtable();
        initialProperties[taskKey] = isTaskAActive;
        PhotonNetwork.CurrentRoom.SetCustomProperties(initialProperties);
    }

    public void ChangeTask(int task)
    {
        if (task == 0)
        {
            isTaskAActive = true;
        }
        else
        {
            isTaskAActive = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            try
            {
                isTaskAActive = (bool)PhotonNetwork.CurrentRoom.CustomProperties[taskKey];
            }
            catch (NullReferenceException)
            {
                //If the property don't exist yet, create it with default values
                Hashtable initialProperties = new Hashtable();
                initialProperties[taskKey] = isTaskAActive;
                PhotonNetwork.CurrentRoom.SetCustomProperties(initialProperties);
            }
        }
    }

    
}