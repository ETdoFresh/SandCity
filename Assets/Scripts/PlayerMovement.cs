using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    Rigidbody _rigidBody;
    NavMeshAgent _navMeshAgent;

    Vector3 _lastHitPoint;
    Vector3 _lastSentHitPoint;
    float _commandTimer;
    public float minimumTimeBetweenCommands = 0.2f;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _rigidBody.isKinematic = isServer ? false : true;
        _navMeshAgent.enabled = isServer ? true : false;

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        _commandTimer = minimumTimeBetweenCommands;
    }

    void FixedUpdate()
    {
        if (isServer)
            NavMeshRigidBodyControl();

        CheckGroundClick();
    }

    [Server]
    void NavMeshRigidBodyControl()
    {
        float mass = _rigidBody.mass;
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
            if (Physics.Raycast(ray, out hit))
                _lastHitPoint = hit.point;
        }

        _commandTimer += Time.fixedDeltaTime;
        if (_lastHitPoint != _lastSentHitPoint && _commandTimer >= minimumTimeBetweenCommands)
        {
            CmdSetDestination(_lastHitPoint);
            _lastSentHitPoint = _lastHitPoint;
            _commandTimer = 0;
        }
    }

    [Command]
    void CmdSetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }
}
