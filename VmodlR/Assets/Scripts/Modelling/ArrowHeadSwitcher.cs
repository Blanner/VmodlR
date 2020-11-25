using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ConnectorTypes
{
    UndirectedAssociation,
    DirectedAssociation,
    Inheritance,
    Aggregation,
    Composition
}

public class ArrowHeadSwitcher : MonoBehaviourPun
{
    #region Public fields

    public ArrowHead associationHead;
    public ArrowHead inheritanceHead;
    public ArrowHead aggregationHead;
    public ArrowHead compositionHead;

    public ArrowHead activeArrowHead;

    #endregion

    #region Monobehaviour Callbacks 

    void Awake()
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

        DisableAllArrowHeads();
        if(activeArrowHead != null)
        {
            activeArrowHead.gameObject.SetActive(true);
        }
    }

    #endregion

    #region public Methods

    public float GetTipDistance()
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

    /// <summary>
    /// Changes the connector type of alle instances of the associated connector across the network
    /// </summary>
    /// <param name="connectorType"></param>
    public void ChangeConnectorType(ConnectorTypes connectorType)
    {
        photonView.RPC("RemoteChangeConnectorType", RpcTarget.All, connectorType);
    }

    #endregion

    #region private Methods

    /// <summary>
    /// Rpc Call that changes the connector type on this local instance of the associated connector. 
    /// Gets called remotely via an rpc call by ChangeConnectorType so the change is synchronized across the network
    /// </summary>
    /// <param name="connectorType"></param>
    [PunRPC]
    private void RemoteChangeConnectorType(ConnectorTypes connectorType)
    {
        DisableAllArrowHeads();

        //set the new active arrow head
        switch (connectorType)
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
        if (activeArrowHead != null)
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

    #endregion
}
