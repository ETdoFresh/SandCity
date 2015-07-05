using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] GameObject _bullet;
    enum MOUSEBUTTON { LEFT, RIGHT, MIDDLE }
    
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

        if (Input.GetMouseButtonDown((int)MOUSEBUTTON.LEFT))
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
