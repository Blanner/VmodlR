using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUIManager : MonoBehaviour
{
    public RectTransform contentTransform;
    public SaveSystem saveSystem;
    public GameObject modelListFileEntryPrefab;
    public GameObject successPanel;
    public GameObject errorPanel;

    public void OnEnable()
    {
        UpdateLoadModelList();
    }

    public void LoadFailed()
    {
        errorPanel.SetActive(true);
    }

    public void LoadSucceeded()
    {
        successPanel.SetActive(true);
    }

    public void LoadModel(string modelName)
    {
        saveSystem.LoadModel(modelName);
    }

    public void UpdateLoadModelList()
    {
        //Destroy all children
        int childCount = contentTransform.childCount;//this will be changing during the loop, so we have to cache it.
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(contentTransform.GetChild(0).gameObject);
        }

        //get all available .xme file's filepaths
#if UNITY_EDITOR
        string[] filePaths = Directory.GetFiles(Application.dataPath.Replace("Assets", ""), "*.xme");
#else
#if UNITY_ANDROID
        string[] filePaths = Directory.GetFiles("/mnt/sdcard/", "*.xme");
#else
        string[] filePaths = Directory.GetFiles(Application.dataPath.Replace("Assets", ""), "*.xme");
#endif
#endif

        //Create new Load UIs
        foreach (string filePath in filePaths)
        {
            GameObject newModelEntry = Instantiate(modelListFileEntryPrefab, contentTransform);
            //get file name
            string[] splitPath = filePath.Split('/');
            string modelName = splitPath[splitPath.Length - 1].Replace(".xme", "");
            newModelEntry.GetComponent<ModelFileUI>().Init(modelName, this);
        }
    }
}
