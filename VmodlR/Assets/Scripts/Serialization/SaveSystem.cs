using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveSystem : MonoBehaviour
{
    public LoadUIManager loadUIManager;

    public GameObject overwritePanel;
    public GameObject errorPanel;
    public Text errorText;
    public GameObject successPanel;
    public InputField fileNameInput;

    private string cannotSaveFileText = "WARNING:\nThe file could not be saved!\n(File System Error)";
    private string modelCannotBeSavedText = "WARNING:\nYou cannot save a model with loose connectors!";

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveModel("savedmodel", true);
        }
    }

    public void OnSaveModelPressed()
    {
        Debug.Log("\nOnSaveModelPressed");
        SaveModel(fileNameInput.text, true);
    }

    public void OnOverwriteFilePressed()
    {
        Debug.Log("\nOnOverwriteSavePressed");
        SaveModel(fileNameInput.text, false);
    }

    private void SaveModel(string path, bool checkExistance)
    {
        Debug.Log($"\nSaveModel: {path}; {checkExistance}");
        if (checkExistance && XMLSerializer.FileExists(path))
        {
            overwritePanel.SetActive(true);
            return;
        }

        ClassSideMirror[] classContents = FindObjectsOfType<ClassSideMirror>();
        Connector[] connections = FindObjectsOfType<Connector>();

        //Check if all classes have unique names
        foreach (ClassSideMirror checkClass in classContents)
        {
            foreach (ClassSideMirror umlClass in classContents)
            {
                if (checkClass != umlClass && umlClass.mirroredSides[0].nameSynchronizer.className == checkClass.mirroredSides[0].nameSynchronizer.className)
                {
                    Debug.LogError($"Cannot save a model with duplicate class names (Class Name: {umlClass.name})");
                    return;
                }
            }
        }

        //create a new model to be saved
        SerialModel model = new SerialModel();
        model.classes = new SerialClass[classContents.Length];
        model.connections = new SerialConnection[connections.Length];

        //Convert each class to a "SerialClass" and each Connector to a SerialConnection
        for (int i = 0; i < classContents.Length; i++)
        {
            SerialClass serialClass = new SerialClass();
            serialClass.className = classContents[i].mirroredSides[0].nameSynchronizer.className;
            serialClass.attributes = new SerialAttribute[classContents[i].mirroredSides[0].fields.classSideElements.Count];
            for (int j = 0; j < serialClass.attributes.Length; j++)
            {
                serialClass.attributes[j] = new SerialAttribute();
                serialClass.attributes[j].content = classContents[i].mirroredSides[0].fields.classSideElements[j].Value;
            }

            serialClass.operations = new SerialOperation[classContents[i].mirroredSides[0].operations.classSideElements.Count];
            for (int j = 0; j < serialClass.operations.Length; j++)
            {
                serialClass.operations[j] = new SerialOperation();
                serialClass.operations[j].content = classContents[i].mirroredSides[0].operations.classSideElements[j].Value;
            }
            model.classes[i] = serialClass;
        }

        try
        {

            //Convert each Connectior to a SerialConnection
            for (int i = 0; i < connections.Length; i++)
            {
                model.connections[i] = new SerialConnection();
                //get connector type
                if (connections[i].arrowHeadManager.activeArrowHead == null)
                {
                    model.connections[i].connectorType = ConnectorTypes.UndirectedAssociation;
                }
                else
                {
                    model.connections[i].connectorType = connections[i].arrowHeadManager.activeArrowHead.ConnectorType;
                }
                //get connector ends
                model.connections[i].originClassName = connections[i].originClass.gameObject.GetComponent<ClassSideMirror>().mirroredSides[0].nameSynchronizer.className;
                model.connections[i].targetClassName = connections[i].targetClass.gameObject.GetComponent<ClassSideMirror>().mirroredSides[0].nameSynchronizer.className;
            }
        }
        catch(NullReferenceException )
        {
            Debug.LogError("You cannot save a model where one or more connectors have an end that is not attached to a class.");
            errorText.text = modelCannotBeSavedText;
            errorPanel.SetActive(true);
            return;
        }

        bool serializationSuccess = XMLSerializer.Serialize(model, path);
        if (!serializationSuccess)
        {
            errorText.text = cannotSaveFileText;
            errorPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"\nSaved model.");
            successPanel.SetActive(true);
            loadUIManager.UpdateLoadModelList();
        }
    }

    public void LoadModel(string modelName)
    {
        try
        {
            string fileName = modelName + ".xme";
#if UNITY_EDITOR
            string filePath = Application.dataPath.Replace("Assets", fileName);
#else
#if UNITY_ANDROID
        string filePath = $"/mnt/sdcard/{fileName}";
#else
        string filePath = Application.dataPath.Replace("Assets", fileName);
#endif
#endif

            SerialModel model = XMLSerializer.Deserialize<SerialModel>(filePath);
            loadUIManager.LoadSucceeded();
            Debug.Log($"\nLoaded Model {filePath}");
        }
        catch(Exception e)
        {
            Debug.LogError("\nError in Load System");
            Debug.LogError("\n" + e.ToString());
            Debug.LogError("\n" + e.StackTrace);
            loadUIManager.LoadFailed();
        }
    }

    private void test()
    {
        SerialClass klausur = new SerialClass();
        klausur.className = "Klausur";
        klausur.attributes = new SerialAttribute[1];
        klausur.attributes[0] = new SerialAttribute();
        klausur.attributes[0].content = "- moduleName : string";

        SerialClass task = new SerialClass();
        task.className = "Task";
        task.attributes = new SerialAttribute[1];
        task.attributes[0] = new SerialAttribute();
        task.attributes[0].content = "- question : string";

        SerialClass calcTask = new SerialClass();
        calcTask.className = "CalculationTask";
        calcTask.attributes = new SerialAttribute[1];
        calcTask.attributes[0] = new SerialAttribute();
        calcTask.attributes[0].content = "- answer : float";
        calcTask.operations = new SerialOperation[2];
        calcTask.operations[0] = new SerialOperation();
        calcTask.operations[0].content = "+ checkAnswer() : bool";
        calcTask.operations[1] = new SerialOperation();
        calcTask.operations[1].content = "+ setAnswer(float) : void";

        SerialClass textTask = new SerialClass();
        textTask.className = "TextTask";
        textTask.attributes = new SerialAttribute[1];
        textTask.attributes[0] = new SerialAttribute();
        textTask.attributes[0].content = "- answer : string";
        textTask.operations = new SerialOperation[1];
        textTask.operations[0] = new SerialOperation();
        textTask.operations[0].content = "+ setAnswer(string) : void";

        SerialConnection inherit1 = new SerialConnection();
        inherit1.connectorType = ConnectorTypes.Inheritance;
        inherit1.originClassName = textTask.className;
        inherit1.targetClassName = task.className;

        SerialConnection inherit2 = new SerialConnection();
        inherit2.connectorType = ConnectorTypes.Inheritance;
        inherit2.originClassName = calcTask.className;
        inherit2.targetClassName = task.className;

        SerialConnection comp = new SerialConnection();
        comp.connectorType = ConnectorTypes.Composition;
        comp.originClassName = task.className;
        comp.targetClassName = klausur.className;

        SerialModel model = new SerialModel();
        model.classes = new SerialClass[4];
        model.classes[0] = klausur;
        model.classes[1] = task;
        model.classes[2] = calcTask;
        model.classes[3] = textTask;
        model.connections = new SerialConnection[3];
        model.connections[0] = inherit1;
        model.connections[1] = inherit2;
        model.connections[2] = comp;

        XMLSerializer.Serialize(model, "model1");
    }
}

