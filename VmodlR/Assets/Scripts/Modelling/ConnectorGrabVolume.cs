using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorGrabVolume : OVRGrabbable
{
    public Connector connector;

    private MeshRenderer meshRenderer;

    private List<GameObject> containedHands = new List<GameObject>();

    private new void Start()
    {
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public void Update()
    {
        if(isGrabbed)
        {
            //This grab volume is grabbed, so it is being moved, so we update the connectors scale and positioning to follow this grab volume
            connector.UpdateTransformFromClassConnections();
        }
    }

    /// <summary>
    /// Updates the ConnectorGrabVolume's position if the volume is not currently grabbed
    /// </summary>
    /// <param name="grabVolumePosition"></param>
    public void UpdateTransform(Vector3 grabVolumePosition)
    {
        if(isGrabbed)
        {
            //if the grab volume is currently grabbed the grab is driving the positioning. Therefore, transform update orders from the connector are ignored
            return;
        }

        transform.position = grabVolumePosition;
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        connector.DetachFromClass(this);
        meshRenderer.enabled = true;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        try
        {
            connector.AttachToClass(this);
        }
        catch(Exception e)
        {
            Debug.Log($"\nCaucht exception: {e.Message}\nat:\n{e.StackTrace}");
            Application.Quit();
        }
        containedHands.Clear();
        meshRenderer.enabled = false;
    }

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("GrabHand"))
        {
            containedHands.Add(other.gameObject);
            meshRenderer.enabled = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("GrabHand"))
        {
            containedHands.Remove(other.gameObject);
            if(containedHands.Count == 0)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}
