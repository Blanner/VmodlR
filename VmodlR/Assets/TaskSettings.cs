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
    

    void Start() //This gets called whenever loading a new scene
    {
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