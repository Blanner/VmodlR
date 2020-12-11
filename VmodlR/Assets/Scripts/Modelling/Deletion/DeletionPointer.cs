using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

[RequireComponent(typeof(LineRenderer))]
public class DeletionPointer : MonoBehaviour
{
    public float maxLength = 20.0f;

    [Tooltip("Gamepad button to enter/exit deletion mode")]
    public OVRInput.Button joyPadToggleButton = OVRInput.Button.One;

    [Tooltip("Gamepad button to enter/exit deletion mode")]
    public OVRInput.Button joyPadDeleteButton = OVRInput.Button.SecondaryIndexTrigger;

    [Tooltip("The Material that gets applied to deletable objects when the deletion pointer is pointed at them")]
    public Material deletionMarkMaterial;

    [Tooltip("The GameObject that marks the start of the Ray")]
    public Transform rayTransform;

    private float hitDistance;

    private LineRenderer lineRenderer;

    private DeletableElement lastHitElement;
    private Material hitStdMaterial;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (OVRInput.GetDown(joyPadToggleButton))
        {
            lineRenderer.enabled = !lineRenderer.enabled;
            //Debug.Log($"\nDeletion mode Active: {lineRenderer.enabled}");
        }

        if (lineRenderer.enabled)
        {
            UpdateRaycast();
            UpdateLaserPointerVisual();

            if (OVRInput.GetDown(joyPadDeleteButton))
            {
                DeleteHitObject();
            }
        }
        else
        {
            ResetDeletionMark();
        }
        
    }

    void OnDisable()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }
    }

    public void ToggleDeletionMode()
    {
        lineRenderer.enabled = false;
    }

    private void UpdateRaycast()
    {
        //cast ray to figure out what object is hit by the laser pointer
        if(Physics.Raycast(rayTransform.position, rayTransform.forward, out RaycastHit hitInfo, maxLength, LayerUtils.getModelElementLayerMask()))
        {
            //update distance to the current hit
            hitDistance = (hitInfo.point - rayTransform.position).magnitude;

            //get the deletable element that belongs to the hit object - if there is one
            DeletableCollider newHitCollider = hitInfo.transform.GetComponent<DeletableCollider>();

            //Pointer points at nothing deletable (anymore)
            if(newHitCollider == null)
            {
                ResetDeletionMark();
                Debug.LogError($"\nGameObject {hitInfo.transform.name} is in Class/Connector Layer but does not have a deletableCollider Compnent Attached");
                return;
            }

            DeletableElement newHitElement = newHitCollider.parentElement;

            if (newHitElement != this.lastHitElement)
            {
                //if we hit something else in the previous frame, reset its state
                ResetDeletionMark();
                //remember the hit element for the next frame
                this.lastHitElement = newHitElement;
                this.hitStdMaterial = newHitElement.deletableRenderer.sharedMaterial;
                //change newHitRenderer's material to the material that marks for deletion
                newHitElement.deletableRenderer.sharedMaterial = deletionMarkMaterial;
            }
        }
        else
        {
            //reset the start and end points of the laser pointer
            hitDistance = maxLength;
            ResetDeletionMark();
        }
        
    }

    private void DeleteHitObject()
    {
        if(lastHitElement != null)
        {
            //delete the element over the network. This has to be done on master since model elements are room objects
            lastHitElement.DeleteOnMaster(lastHitElement.photonView.ViewID);
            lastHitElement = null;
            hitStdMaterial = null;
        }
    }

    private void UpdateLaserPointerVisual()
    {
        lineRenderer.SetPosition(0, rayTransform.position);
        lineRenderer.SetPosition(1, rayTransform.position + hitDistance * rayTransform.forward);
    }

    private void ResetDeletionMark()
    {
        if (lastHitElement != null)
        {
            //if we hit something deletable in the last frame, reset the state of that element
            this.lastHitElement.deletableRenderer.sharedMaterial = hitStdMaterial;
            this.lastHitElement = null;
            hitStdMaterial = null;
        }
    }
}
