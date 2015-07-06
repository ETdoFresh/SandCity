using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] GameObject _bullet;    
    Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        SendInput();
    }

    [Client]
    void SendInput()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetButtonDown("Fire2"))
            CmdPlayerShoot();
    }

    [Command]
    void CmdPlayerShoot()
    {
        GameObject bullet = (GameObject)Instantiate(_bullet, transform.position, transform.rotation);
        NetworkServer.Spawn(bullet);
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(bulletCollider, playerCollider);
    }
}
