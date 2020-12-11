using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

/// <summary>
/// Handles the visual representation of a Class. 
/// Connectors can be attached to this class and their transforms will be updated to stick to this class then the class is being moved.
/// </summary>
public class UMLClass : MonoBehaviourPun, IGrabListener
{
    public List<Connector> connectors;

    private bool isMoving = false;

    #region MonoBehaviour Callbacks

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            UpdateConnectorTransforms();
        }
    }

    void OnDestroy()
    {
        //On destory is called locally on every client when we delete this object over the network.
        //This means we only have to clean up the attach state locally but we have to make sure the latest attach event is properly updated
        Debug.Log($"\nOnDestory called on GO {gameObject.name}");
        //Detach the connector from all connected classes locally on this client
        //The ToArray() call is vital because OnDestroyAttachedClass will mofiy the connectors List, in order to go over every connector exactly one we copy the list to an array
        foreach (Connector connector in connectors.ToArray())
        { 
            connector.OnDestroyAttachedClass(this);
        }
    }

    #endregion


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
