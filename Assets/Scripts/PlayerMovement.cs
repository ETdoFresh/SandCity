using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    public float movementAxisScale = 20f;
    public float rotationAxisScale = 4f;
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

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            CmdPlayerMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    [Command]
    void CmdPlayerMove(float horizontal, float vertical)
    {
        _rigidBody.AddRelativeForce(Vector3.forward * vertical * movementAxisScale);
        _rigidBody.AddRelativeTorque(Vector3.up * horizontal * rotationAxisScale);
    }
}
