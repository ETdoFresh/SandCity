using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    public enum NavigationMode { NAVMESH_ONLY, NAVMESH_RIGIDBODY }

    Rigidbody _rigidBody;
    NavMeshAgent _navMeshAgent;
    float _slowRadius; // At full speed, what is the stopping distance

    public NavigationMode navigationMode = NavigationMode.NAVMESH_ONLY;
    public float RigidbodyVelocity;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _rigidBody.isKinematic = isServer ? false : true;
        _navMeshAgent.enabled = isServer ? true : false;

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
        SelectMovementMode();
        CheckGroundClick();
    }

    [Server]
    void SelectMovementMode()
    {
        switch (navigationMode)
        {
            case NavigationMode.NAVMESH_ONLY:
                if (!_rigidBody.isKinematic)
                    _rigidBody.isKinematic = true;
                if (!_navMeshAgent.updatePosition)
                    _navMeshAgent.updatePosition = true;
                break;
            case NavigationMode.NAVMESH_RIGIDBODY:
                if (_rigidBody.isKinematic)
                    _rigidBody.isKinematic = false;
                if (_navMeshAgent.updatePosition)
                    _navMeshAgent.updatePosition = false;
                NavMeshRigidBodyControl();
                break;
        }
    }

    [Server]
    void NavMeshRigidBodyControl()
    {
        RigidbodyVelocity = _rigidBody.velocity.magnitude;
        float mass = _rigidBody.mass;
        float acceleration = _navMeshAgent.acceleration;
        float maxspeed = _navMeshAgent.speed;
        float currentspeed = _rigidBody.velocity.magnitude;
        Vector3 changeInVelocity = _navMeshAgent.desiredVelocity - _rigidBody.velocity;
        changeInVelocity.y = 0;

        if (changeInVelocity.magnitude > _navMeshAgent.acceleration)
            changeInVelocity = changeInVelocity.normalized * _navMeshAgent.acceleration;

        _rigidBody.AddForce(mass * changeInVelocity / Time.fixedDeltaTime);

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
