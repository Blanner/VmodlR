using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas), typeof(OVRRaycaster))]
public class ClassSide : MonoBehaviour
{
    public string cameraGOName = "CenterEyeAnchor";
    public string UIHelperGOName = "UIHelpers";

    public SideMirror mirror;

    public InputField className;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(TagUtils.localPlayerTag);
        Transform camera = player.transform.FindChildRecursive(cameraGOName);
        GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();

        GetComponent<OVRRaycaster>().pointer = player.transform.FindChildRecursive(UIHelperGOName).gameObject;
    }

    public void OnChangedName(string newName)
    {
        mirror.OnChangedName(newName);
    }

    public void ChangeName(string newName)
    {
        className.SetTextWithoutNotify(newName);
    }
}
