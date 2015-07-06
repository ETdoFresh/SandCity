using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    Rigidbody _rigidBody;
    NavMeshAgent _navMeshAgent;
    float _slowRadius; // At full speed, what is the stopping distance

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _rigidBody.isKinematic = isServer ? false : true;

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        float initialVelocity = _navMeshAgent.speed;
        float finalVelocity = 0;
        float acceleration = -_navMeshAgent.acceleration; // aka deceleration
        float timeToStop = (finalVelocity - initialVelocity) / acceleration;
        _slowRadius = (initialVelocity * timeToStop) + (1 / 2) * (acceleration * Mathf.Pow(timeToStop, 2));
    }

    void FixedUpdate()
    {
        NavMeshRigidBodyControl();
        CheckGroundClick();
    }

    [Server]
    void NavMeshRigidBodyControl()
    {
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) > _slowRadius)
            _rigidBody.AddForce(_navMeshAgent.velocity * 5);
        else
        {
            _rigidBody.velocity = _navMeshAgent.velocity;
        }
        _navMeshAgent.nextPosition = transform.position + Time.deltaTime * _rigidBody.velocity;
    }

    [Client]
    void CheckGroundClick()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = ~(1 << LayerMask.NameToLayer("Ground"));
            if (Physics.Raycast(ray, out hit))
                CmdSetDestination(hit.point);
        }
    }

    [Command]
    void CmdSetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }
}
