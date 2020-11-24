
using UnityEngine;
using UnityEngine.UI;


public class MenuToggle : MonoBehaviour
{
    public Text buttonText;
    public GameObject targetMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonText == null)
        {
            Debug.LogError("Menu Toggle's Button text is null!");
        }
        if (targetMenu == null)
        {
            Debug.LogError("Menu Toggle's targetMenu is null!");
        }

        if(targetMenu != null && buttonText != null)
        {
            SetMenuActive(false);
        }
    }

    public void ToggleMenu()
    {
        SetMenuActive(!targetMenu.activeSelf);
    }

    private void SetMenuActive(bool active)
    {
        targetMenu.SetActive(active);

        if (targetMenu.activeSelf)
        {
            buttonText.text = "<";
        }
        else
        {
            buttonText.text = ">";
        }
    }
}
