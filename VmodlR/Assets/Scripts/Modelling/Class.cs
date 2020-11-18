using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : NetworkModelElement
{
    public List<Connector> connectors;

    private bool isMoving = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if(IsMine())
        {
            UpdateConnectorPositions();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            UpdateConnectorPositions();
        }
    }

    public void AddConnector(Connector connector)
    {
        connectors.Add(connector);
    }

    public void RemoveConnector(Connector connector)
    {
        connectors.Remove(connector);
    }

    public void BeginMovement()
    {
        Debug.LogWarning("Beginning Movement");
        RequestOwnership();
        isMoving = true;
    }

    public void EndMovement()
    {
        isMoving = false;
    }

    private void UpdateConnectorPositions()
    {
        foreach (Connector connector in connectors)
        {
            connector.UpdateTransformFromClassConnections();
        }
    }
}
