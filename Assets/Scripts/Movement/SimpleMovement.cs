using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    Rigidbody _rigidBody;
    NavMeshAgent _navMeshAgent;
    Animator _animator;
    float _animatorAverageVelocity;

    public float rigidBodyVelocity;
    public float animatorDeltaPosition;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
                
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        _animator = GetComponent<Animator>();
        _animator.SetFloat("Speed", 1);
        _animatorAverageVelocity = 0.574f;
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                _navMeshAgent.SetDestination(hit.point);
        }

        NavMeshRigidBodyControl();
    }

    void NavMeshRigidBodyControl()
    {
        _animator.speed = _navMeshAgent.speed * _animatorAverageVelocity;

        float mass = _rigidBody.mass;
        Vector3 changeInVelocity = _navMeshAgent.desiredVelocity - _rigidBody.velocity;
        changeInVelocity.y = 0;

        if (changeInVelocity.magnitude > _navMeshAgent.acceleration)
            changeInVelocity = changeInVelocity.normalized * _navMeshAgent.acceleration;

        _rigidBody.AddForce(mass * changeInVelocity / Time.fixedDeltaTime);

        _navMeshAgent.nextPosition = transform.position + Time.deltaTime * _rigidBody.velocity;

        rigidBodyVelocity = _rigidBody.velocity.magnitude;
        animatorDeltaPosition = _animator.deltaPosition.magnitude;
        _animator.SetFloat("Speed", _rigidBody.velocity.magnitude);
        //_animator.speed = _rigidBody.velocity.magnitude * _animationNormalizeSpeed;
    }
}