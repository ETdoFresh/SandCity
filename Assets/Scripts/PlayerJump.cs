using UnityEngine;
using UnityEngine.Networking;

public class PlayerJump : NetworkBehaviour
{
    public float jumpAxisScale = 20f;
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
        if (isLocalPlayer && Input.GetAxis("Jump") > 0)
            CmdJump(Input.GetAxis("Jump"));
    }

    [Command]
    void CmdJump(float jump)
    {
        _rigidBody.AddRelativeForce(Vector3.up * jump * jumpAxisScale);
    }
}
