using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCanvasInitializer : MonoBehaviour
{
    public string cameraGOName = "CenterEyeAnchor";
    public string UIHelperGOName = "UIHelpers";

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(TagUtils.localPlayerTag);
        Transform camera = player.transform.FindChildRecursive(cameraGOName);
        GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();

        GetComponent<OVRRaycaster>().pointer = player.transform.FindChildRecursive(UIHelperGOName).gameObject;
    }
}
