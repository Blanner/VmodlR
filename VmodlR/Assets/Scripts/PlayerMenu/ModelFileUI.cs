using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelFileUI : MonoBehaviour
{
    public Text modelNameText;
    private LoadUIManager loadManager;

    public void Init(string modelName, LoadUIManager loadManager)
    {
        modelNameText.text = modelName;
        this.loadManager = loadManager;
    }

    public void LoadModel()
    {
        loadManager.LoadModel(modelNameText.text);
    }
}
