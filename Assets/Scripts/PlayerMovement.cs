using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    public float movementAxisScale = 20f;
    public float rotationAxisScale = 4f;
    public float jumpAxisScale = 20f;

    Rigidbody _rigidBody;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        if (Component.FindObjectOfType<NetworkManager>() != null)
            Debug.Log("Found Network Manager");
        else
            Debug.Log("Did not find Network Manager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SendInput();
    }

    [Client]
    void SendInput()
    {
        if (!isLocalPlayer)
            return;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Input.GetAxis("Jump") > 0)
            CmdAdjustForce(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Jump"));
    }

    [Command]
    void CmdAdjustForce(float horizontal, float vertical, float jump)
    {
        _rigidBody.AddRelativeForce(Vector3.forward * vertical * movementAxisScale);
        _rigidBody.AddRelativeTorque(Vector3.up * horizontal * rotationAxisScale);
        _rigidBody.AddRelativeForce(Vector3.up * jump * jumpAxisScale);
    }
}
