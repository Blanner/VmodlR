using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ModelElementSpawner : MonoBehaviourPun
{
    #region Public Fields

    public string classPrefabName;
    public string undirectedAssociationPrefabName;
    public string directedAssociationPrefabName;
    public string InheritancePrefabName;
    public string AggregationPrefabName;
    public string CompositionPrefabName;

    #endregion

#if DEBUG

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SpawnClass();
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            SpawnUndirectedAssociation();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            SpawnDirectedAssociation();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnInheritance();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnAggregation();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnComposition();
        }
    }

#endif

    #region Public Methods

    public void SpawnUndirectedAssociation()
    {
        SpawnConnector(undirectedAssociationPrefabName);
    }

    public void SpawnDirectedAssociation()
    {
        SpawnConnector(directedAssociationPrefabName);
    }

    public void SpawnInheritance()
    {
        SpawnConnector(InheritancePrefabName);
    }

    public void SpawnAggregation()
    {
        SpawnConnector(AggregationPrefabName);
    }

    public void SpawnComposition()
    {
        SpawnConnector(CompositionPrefabName);
    }

    public void SpawnClass()
    {
        InstantiateOnMaster($"Prefabs/ModelElements/{classPrefabName}", transform.position + 2 * calculateHorizontalForward() + 0.5f * Vector3.up,
                Quaternion.LookRotation(calculateHorizontalForward(), Vector3.up), 0);
        
    }

    #endregion

    #region Private Methods

    private void SpawnConnector(string prefabName)
    {
        InstantiateOnMaster($"Prefabs/ModelElements/{prefabName}", transform.position + 0.5f * calculateHorizontalForward() + 0.5f * calculateHorizontalRight(), 
            Quaternion.LookRotation(calculateHorizontalForward(), Vector3.up), 0);
    }

    /// <summary>
    /// Instantiates a prefab as a room object. This ensured to be only done on the masterclient 
    /// by either doing it localy if this is the master client or calling this method as an RPC on the master client otherwise. 
    /// </summary>
    [PunRPC]
    private void InstantiateOnMaster(string prefabPath, Vector3 position, Quaternion rotation, byte group = 0)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Instantiate Room Object can only be called on the master client according to PUN documentation
            //This function prevents objects from being deleted when the user that originally spawned them leaves the room
            PhotonNetwork.InstantiateRoomObject(prefabPath, position, rotation, group);
        }
        else
        {
            photonView.RPC("InstantiateOnMaster", RpcTarget.MasterClient, prefabPath, position, rotation, group);
        }
    }

    private Vector3 calculateHorizontalForward()
    {
        return Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    }

    private Vector3 calculateHorizontalRight()
    {
        return Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
    }

    #endregion
}
