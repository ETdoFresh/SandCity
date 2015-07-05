using UnityEngine;
using System.Collections;

public class BulletShoot : MonoBehaviour
{
    public float shootForce = 20f;

    void Awake()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddRelativeForce(Vector3.forward * shootForce, ForceMode.Impulse);
    }
}
