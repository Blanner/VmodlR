using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text tutorialContent;
    public Button closeButton;

    private int currMsg = 0;

    private string[] tutorialMessages = 
        { 
          "Press the '+'-button on the menu below\nby pointing your right hand at it and pressing the index finger button on your right controller.\nA white ball marks the spot your right hand points at.", 
          "Create a new Class in front of you\nby pressing the 'Class'-button on the newly opened menu\n(in the same way as the '+'-button before).",
          "Move towards the Class.\nIf you want to move without walking in real life,\nyou can move your character with the control stick next to your left thumb.",
          "You can also fly up or down by moving the controll stick on your right controller up and down.\nMoving this stick left or right will turn you sideways.",
          "Use the button at your middle and ring fingers to grab with your hand.\nTry it out by looking at your hands.",
          "Put your hand into the Class\nand grab it to move it around.",
          "Back off again, so you can see one side of the Class completely.",
          "You can toggle a pink laser pointer\nby pressing the upper one of the two buttons beneath your right thumb.\nOnly you can see this laser pointer.\nYou can use it to help aim at UI elements.",
          "Aim at the 'Class Name' and press your right index finger button to open the keyboard.\nDelete the old name, then enter 'Test'.\nClose the keyboard again using its enter button on the right.",
          "Aim at the space at the right of the grey separation lines of the Class body.\nPress the appearing green buttons or the lines next to them to add an attribute or operation at the respective position.",
          "You can enter text in the same way that you entered a Class name.\nYou do not need to enter something now.\nYou can proceed.",
          "Now create a second Class and move it next to the first Class.\nMake sure there is enough space between them to insert a connection.",
          "Add a connection via one of the buttons next to the 'Class'-button in the menu below.\nThe connection will appear approximately one meter in front of you.",
          "You can grab either end of the connection to move this end, like you grabbed the Class.\nA grey sphere will appear when your hand touches a grabbable end of the connection.\nProceed once you can see the grey sphere.",
          "Grab one of the connection ends and aim it at the side of a class that faces the other class.\nLet loose the connection to attach it to the class.\nYour connection end does not have to touch the class, you only need to aim it at the class.",
          "Attach the other connection end to the other class.",
          "Disable the pink UI laser pointer if it is still active.\n\nOn your right controller, press the lower button beneath your thumb to toggle the red deletion laser pointer.",
          "Point the deletion laser pointer at one of the classes you just created.\nPress the right index finger button to delete the marked object.",
          "Delete both classes and the connection you created."
        };

    // Start is called before the first frame update
    void Start()
    {
        if(tutorialContent != null)
        {
            tutorialContent.text = tutorialMessages[currMsg];
        }
        else
        {
            Debug.LogError("Tutorial does not have a content Text assigned!");
            this.enabled = false;
        }

        if(closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Tutorial close button is not assigned!");
            this.enabled = false;
        }
    }

    public void ShowNextTutorialMsg()
    {
        currMsg++;

        if(currMsg < tutorialMessages.Length)
        {
            tutorialContent.text = tutorialMessages[currMsg];
        }
        else
        {
            tutorialContent.text = "Congratulations, you learned all essential controls!\nPlease complete your task together with your partner now."
                                 + "\nThe task description can be opened by pressing the button left of the '+'-button in the menu below";
            closeButton.gameObject.SetActive(true);
        }
    }

    public void ShowPrevTutorialMsg()
    {
        currMsg--;

        if (currMsg < 0)
        {
            currMsg = 0;
        }

        tutorialContent.text = tutorialMessages[currMsg];
    }
}
