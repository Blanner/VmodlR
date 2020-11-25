using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual representation of a Class. 
/// Connectors can be attached to this class and their transforms will be updated to stick to this class then the class is being moved.
/// </summary>
public class UMLClass : MonoBehaviour, IGrabListener
{
    public List<Connector> connectors;

    private bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            UpdateConnectorTransforms();
        }
    }


    /// <summary>
    /// Adds the given connector to the list of connectors attached to this class.
    /// </summary>
    /// <param name="connector"></param>
    public void AddConnector(Connector connector)
    {
        connectors.Add(connector);
    }

    public void RemoveConnector(Connector connector)
    {
        connectors.Remove(connector);
    }

    /// <summary>
    /// Makes the class ready to be moved. Connectors attached to this class will now update their positions, scale etc. to stick to the class while moving
    /// </summary>
    public void OnGrabBegin()
    {
        Debug.LogWarning("Beginning Movement");
        isMoving = true;
    }

    /// <summary>
    /// Signals the end of a movement, so attached connectors do not have to be updated every frame anymore
    /// </summary>
    public void OnGrabEnd()
    {
        isMoving = false;
    }

    /// <summary>
    /// Updates the transforms of attached connectors to stick to this class.
    /// </summary>
    private void UpdateConnectorTransforms()
    {
        foreach (Connector connector in connectors)
        {
            connector.UpdateTransform();
        }
    }
}
