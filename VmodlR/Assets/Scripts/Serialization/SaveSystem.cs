using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

        XMLSerializer.Serialize(model, "model.xml");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveModel("savedmodel.xml");
        }
    }

    public void SaveModel(string path)
    {
        ClassSideMirror[] classContents = FindObjectsOfType<ClassSideMirror>();
        Connector[] connections = FindObjectsOfType<Connector>();

        //Check if all classes have unique names
        foreach (ClassSideMirror checkClass in classContents)
        {
            foreach (ClassSideMirror umlClass in classContents)
            {
                if (umlClass.mirroredSides[0].nameSynchronizer.className == checkClass.mirroredSides[0].nameSynchronizer.className)
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
            serialClass.attributes = new SerialAttribute[classContents[0].mirroredSides[0].fields.classSideElements.Count];
            for (int j = 0; j < serialClass.attributes.Length; j++)
            {
                serialClass.attributes[j] = new SerialAttribute();
                serialClass.attributes[j].content = classContents[0].mirroredSides[0].fields.classSideElements[j].Value;
            }

            serialClass.operations = new SerialOperation[classContents[0].mirroredSides[0].operations.classSideElements.Count];
            for (int j = 0; j < serialClass.operations.Length; j++)
            {
                serialClass.operations[j] = new SerialOperation();
                serialClass.operations[j].content = classContents[0].mirroredSides[0].operations.classSideElements[j].Value;
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
            return;
        }

        XMLSerializer.Serialize(model, path);
        Debug.Log($"\nSaved model under: {Application.dataPath.Replace("Assets", path)}");
    }

    public void loadModel(string path)
    {
        SerialModel model = XMLSerializer.Deserialize<SerialModel>(path);
    }
}

