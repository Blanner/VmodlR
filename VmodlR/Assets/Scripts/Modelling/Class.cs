using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : MonoBehaviour
{
    public List<Connector> connectors;

    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateConnectorPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            UpdateConnectorPositions();
        }
    }


    public void BeginMovement()
    {
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
            connector.UpdatePosition();
        }
    }
}
