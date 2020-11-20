using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Volume is a trigger collider that sits at one end of a connector. 
/// It is responsible to capture the grab input of the player and telling the connector when it needs to detach/attach to a class and when it has to move based on the player grabbing and moving this volume.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public class ConnectorGrabVolume : MonoBehaviour, IGrabListener
{
    /// <summary>
    /// The connector this Grab volume belongs to
    /// </summary>
    public Connector connector;

    /// <summary>
    /// The renderer responsible for rendering the GrabVolume, when a Hand enters it.
    /// </summary>
    private MeshRenderer meshRenderer;

    /// <summary>
    /// A list that contains all player hands that the grab volume currently contains. 
    /// The GrabVolume is hidden when this list is empty and visible if the list has at least one element.
    /// </summary>
    private List<GameObject> containedHands = new List<GameObject>();

    private bool isGrabbed = false;

    private new void Start()
    {
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

    public void OnGrabBegin()
    {
        isGrabbed = true;
        connector.DetachFromClass(this);
        meshRenderer.enabled = true;
    }

    public void OnGrabEnd()
    {
        isGrabbed = false;
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

    /// <summary>
    /// Adds the entering game object to a list of hands, if the entered collider belongs to a GrabHand and shows the visual representation of the GrabVolume
    /// </summary>
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("GrabHand"))
        {
            containedHands.Add(other.gameObject);
            meshRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Removes the entering game object from the list of hands, if the entered collider belongs to a GrabHand 
    /// And hides the visual representation of the GrabVolume if there is no other hand still in the grabVolume.-
    /// </summary>
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
