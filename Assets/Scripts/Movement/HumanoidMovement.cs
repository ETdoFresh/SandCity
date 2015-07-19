using UnityEngine;
using System.Collections;

public class HumanoidMovement : MonoBehaviour
{
    [SerializeField]
    float m_GroundCheckDistance = 0.2f;
    [SerializeField]
    float m_MoveSpeedMultiplier = 1;
    [SerializeField]
    Vector3 m_Target;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;
    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_GravityMultiplier = 2;
    [SerializeField]
    float m_JumpPower = 6;

    Rigidbody _rigidBody;
    Animator _animator;
    NavMeshAgent _navMeshAgent;

    Vector3 m_GroundNormal;
    float m_TurnAmount;
    float m_ForwardAmount;
    bool m_IsGrounded;
    bool m_Crouching;
    float m_OrigGroundCheckDistance;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        m_GroundNormal = Vector3.up;
        m_TurnAmount = 0;
        m_ForwardAmount = 0;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                _navMeshAgent.SetDestination(hit.point);
        }

        bool jump = Input.GetButtonDown("Jump");
        bool crouch = Input.GetKey(KeyCode.C);

        MoveUpdate(_navMeshAgent.desiredVelocity, jump, crouch);
    }

    void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (_animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            _navMeshAgent.nextPosition = transform.position;

            transform.position += _animator.deltaPosition;
            transform.rotation *= _animator.deltaRotation;

            // we preserve the existing y part of the current velocity.
            //v.y = _rigidBody.velocity.y;
            //_rigidBody.velocity = v;
        }
    }

    public void MoveUpdate(Vector3 move, bool jump = false, bool crouch = false)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        if (m_IsGrounded)
            HandleGroundedMovement(crouch, jump);
        else
            HandleAirborneMovement();
    }

    private void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        _rigidBody.AddForce(extraGravityForce);

        m_GroundCheckDistance = _rigidBody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }

    private void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && _animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, m_JumpPower, _rigidBody.velocity.z);
            m_IsGrounded = false;
            _animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }

    private void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            _animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            _animator.applyRootMotion = false;
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        // update the animator parameters
        _animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        _animator.SetBool("Crouch", m_Crouching);
        _animator.SetBool("OnGround", m_IsGrounded);
        if (!m_IsGrounded)
        {
            _animator.SetFloat("Jump", _rigidBody.velocity.y);
        }

        _animator.speed = _navMeshAgent.speed / 5.661f;
    }
}
