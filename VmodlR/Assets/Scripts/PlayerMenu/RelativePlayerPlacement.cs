using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePlayerPlacement : MonoBehaviour
{
    public Vector3 PositionRelativToTarget = new Vector3(0, 1.35f, 2);
    public float maxPosDistance = .5f;
    public Vector3 BaseRotation = new Vector3(0, 0, 0);

    public float smoothTime = 2;

    [Tooltip("The Transform this gameObject should be place relative to. Defaults to the first Object found with Tag 'localPlayer'")]
    public Transform target;

    private Quaternion directionRotation = Quaternion.identity;

    private Vector3 smoothVelocity;
    private bool smoothing = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(TagUtils.localPlayerTag).transform;
        }
       
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
    }

    void Update()
    {
        float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized, Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized, Vector3.up);
        
        if(angle > 60)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            forward = Quaternion.Euler(0, 90, 0) * forward;
            directionRotation = Quaternion.LookRotation(forward);
            transform.rotation = directionRotation * Quaternion.Euler(BaseRotation);
        }
        else if (angle < -60)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            forward = Quaternion.Euler(0, -90, 0) * forward;
            directionRotation = Quaternion.LookRotation(forward);
            transform.rotation = directionRotation * Quaternion.Euler(BaseRotation);
        }

        Vector3 desiredPosition = target.position + directionRotation * PositionRelativToTarget;
        if (smoothing || Vector3.Distance(desiredPosition, transform.position) >= maxPosDistance)
        {
            smoothing = true;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, smoothTime * Time.deltaTime);
            if(Vector3.Distance(transform.position, desiredPosition) < 0.01f)
            {
                smoothing = false;
            }
        }
    }
}
