using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPool : MonoBehaviour
{
    [TextArea(3, 10)]
    public string taskAGerman = "";
    [TextArea(3, 10)]
    public string taskAEnglish = "";
    [TextArea(3, 10)]
    public string taskBGerman = "";
    [TextArea(3, 10)]
    public string taskBEnglish = "";

    public Text taskContent;
    public Text taskLabel;

    private bool isTaskAActive = true;
    private bool isLanguageGerman = true;

    void Start()
    {
        setTaskActive(FindObjectOfType<TaskSettings>().isTaskAActive);
    }

    private void setTaskActive(bool taskA)
    {
        if(taskA)
        {
            ShowTaskA();
        }
        else
        {
            ShowTaskB();
        }
    }

    public void ChangeLanguage(int language)
    {
        if (language == 0)//German
        {
            isLanguageGerman = true;
        }
        else //English
        {
            isLanguageGerman = false;
        }

        if(isTaskAActive)
        {
            ShowTaskA();
        }
        else
        {
            ShowTaskB();
        }
    }

    private void ShowTaskA()
    {
        isTaskAActive = true;
        if(isLanguageGerman)
        {
            taskLabel.text = "Aufgabe";
            taskContent.text = taskAGerman;
        }
        else
        {
            taskLabel.text = "Task";
            taskContent.text = taskAEnglish;
        }
    }

    private void ShowTaskB()
    {
        isTaskAActive = false;
        if (isLanguageGerman)
        {
            taskLabel.text = "Aufgabe";
            taskContent.text = taskBGerman;
        }
        else
        {
            taskLabel.text = "Task";
            taskContent.text = taskBEnglish;
        }
    }
}
