using UnityEngine;

using Photon.Pun;

public class DeletableCollider : MonoBehaviour
{
    public DeletableElement parentElement;

    // Start is called before the first frame update
    void Start()
    {
        if (parentElement == null)
        {
            Debug.LogError($"Parent Element of DeletableElement on {gameObject.name} is not set!");
            this.enabled = false;
        }
    }
}
