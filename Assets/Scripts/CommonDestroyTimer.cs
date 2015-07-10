using UnityEngine;

public class CommonDestroyTimer : MonoBehaviour
{
    public float secondsBeforeDestroy;

    void Awake()
    {
        Destroy(gameObject, secondsBeforeDestroy);
    }
}