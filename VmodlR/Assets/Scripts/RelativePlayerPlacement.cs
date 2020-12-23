using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePlayerPlacement : MonoBehaviour
{
    public Vector3 PositionRelativToTarget = new Vector3(0, 1.35f, 2);
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(TagUtils.localPlayerTag).transform;
       
        if (target == null)
        {
            Debug.LogError("RelativePlayerPlacementScript could not find local player!");
            this.enabled = false;
            return;
        }

        //Attach to the camera Rigs eye position if the target is a lone camera rig. 
        //Otherwise this is attached to a complete VR Character, which means the target should move with the camera via a CharacterControllerCameraConstraint script.
        OVRCameraRig attachedCameraRig = target.GetComponent<OVRCameraRig>();
        if(attachedCameraRig != null)
        {
            target = attachedCameraRig.centerEyeAnchor;
            if(target == null)
            {
                Debug.LogError("\nCenter Eye Anchor is nulL!");
                this.enabled = false;
            }
        }

        Debug.Log($"\nTarget Position: {target.position}");
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized, Vector3.up);
        
        if(angle > 60)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            forward = Quaternion.Euler(0, 90, 0) * forward;
            transform.rotation = Quaternion.LookRotation(forward);
        }
        else if (angle < -60)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            forward = Quaternion.Euler(0, -90, 0) * forward;
            transform.rotation = Quaternion.LookRotation(forward);
        }

        transform.position = target.position + transform.rotation * PositionRelativToTarget;
    }
}
