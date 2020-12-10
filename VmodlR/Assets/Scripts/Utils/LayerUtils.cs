using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtils
{
    public static string ClassLayer = "Class";
    public static string ConnectorLayer = "Connector";

    public static bool IsModelElementCollider(Collider hitCollider)
    {
        return hitCollider.gameObject.layer == LayerMask.NameToLayer(ClassLayer) || hitCollider.gameObject.layer == LayerMask.NameToLayer(ConnectorLayer);
    }

    public static int getModelElementLayerMask()
    {
        return (1 << LayerMask.NameToLayer(ClassLayer)) | (1 << LayerMask.NameToLayer(ConnectorLayer));
    }
}
