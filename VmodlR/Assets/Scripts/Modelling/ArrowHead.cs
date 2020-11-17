using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHead : MonoBehaviour
{
    [Tooltip("The distance between the tip of the arrow and the point where the connector this arrow head sits on should end")]
    public float TipDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
