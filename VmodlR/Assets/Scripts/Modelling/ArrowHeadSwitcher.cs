using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHeadSwitcher : MonoBehaviour
{
    public enum ConnectorTypes
    {
        UndirectedAssociation,
        DirectedAssociation,
        Inheritance,
        Aggregation,
        Composition
    }

    public ArrowHead associationHead;
    public ArrowHead inheritanceHead;
    public ArrowHead aggregationHead;
    public ArrowHead compositionHead;

    public ArrowHead activeArrowHead { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if(associationHead == null)
        {
            Debug.LogError($"Association Head of Connector{gameObject.name} is null!");
        }
        if (inheritanceHead == null)
        {
            Debug.LogError($"Inheritance Head of Connector{gameObject.name} is null!");
        }
        if (aggregationHead == null)
        {
            Debug.LogError($"Aggregation Head of Connector{gameObject.name} is null!");
        }
        if (compositionHead == null)
        {
            Debug.LogError($"Composition Head of Connector{gameObject.name} is null!");
        }
    }

    public float getTipDistance()
    {
        if (activeArrowHead != null)
        {
            return activeArrowHead.TipDistance;
        }
        else
        {
            return 0;
        }
    }

    public void changeConnectorType(ConnectorTypes connectorType)
    {
        DisableAllArrowHeads();

        //set the new active arrow head
        switch(connectorType)
        {
            case ConnectorTypes.UndirectedAssociation:
                activeArrowHead = null;
                break;
            case ConnectorTypes.DirectedAssociation:
                activeArrowHead = associationHead;
                break;
            case ConnectorTypes.Inheritance:
                activeArrowHead = inheritanceHead;
                break;
            case ConnectorTypes.Aggregation:
                activeArrowHead = aggregationHead;
                break;
            case ConnectorTypes.Composition:
                activeArrowHead = compositionHead;
                break;
        }

        //enable the visual of the new connector type (if it is not an undirected association, which has no arrow head)
        if(activeArrowHead != null)
        {
            activeArrowHead.gameObject.SetActive(true);
        }
    }

    private void DisableAllArrowHeads()
    {
        associationHead.gameObject.SetActive(false);
        inheritanceHead.gameObject.SetActive(false);
        aggregationHead.gameObject.SetActive(false);
        compositionHead.gameObject.SetActive(false);
    }
}
