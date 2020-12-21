
using UnityEngine;
using UnityEngine.UI;


public class MenuToggle : MonoBehaviour
{
    public Text buttonText;
    public GameObject targetMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (targetMenu == null)
        {
            Debug.LogError("Menu Toggle's targetMenu is null!");
        }

        if(targetMenu != null)
        {
            SetMenuActive(false);
        }
    }

#if DEBUG

    void Update()
    {
        //This is only needed for debug purposes in the editor
        if(Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

#endif

    public void ToggleMenu()
    {
        SetMenuActive(!targetMenu.activeSelf);
    }

    private void SetMenuActive(bool active)
    {
        targetMenu.SetActive(active);

        if(buttonText != null)
        {
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
}
