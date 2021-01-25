/* Copyright Notice
 * 
 * This script was kindly provided by 
 * 
 * Sebastian Krois
 * Research Group Databases and Information Systems
 * University of Paderborn, Germany
 * 
 * for use in this application.
 * All rights belong to him.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudColliderPlacement : MonoBehaviour
{
    public GameObject hudObject;

    private BoxCollider hudCollider;

    void Start()
    {
        hudCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {

        if (!hudCollider.bounds.Contains(hudObject.transform.position))
        {
            hudObject.transform.position = hudCollider.bounds.ClosestPoint(hudObject.transform.position);
            hudObject.transform.LookAt(transform.parent);
            var rotation = hudObject.transform.rotation;
            rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z));
            hudObject.transform.rotation = rotation;
        }



        transform.LookAt(transform.parent);
        var thisRotation = transform.rotation;
        thisRotation = Quaternion.Euler(new Vector3(0, thisRotation.eulerAngles.y, thisRotation.eulerAngles.z));
        transform.rotation = thisRotation;
    }
}
