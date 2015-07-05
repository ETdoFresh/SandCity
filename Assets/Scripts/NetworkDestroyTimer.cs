using UnityEngine;
using UnityEngine.Networking;

public class NetworkDestroyTimer : NetworkBehaviour
{
    public float secondsBeforeDestroy;

    void Awake()
    {
        Destroy(gameObject, secondsBeforeDestroy);
    }
}