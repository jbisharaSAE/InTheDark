using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovementComp : MonoBehaviour
{
    [SerializeField, Min(0f)] private float m_maxSpeed = 10f;
    [SerializeField, Min(0f)] private float m_walkAcceleration = 20f;
    [SerializeField, Min(0f)] private float m_brakeFriction = 2.5f;
    [SerializeField, Range(0f, 1f)] private float m_airSpeedMultiplier = 0.8f;
    [SerializeField, Min(0f)] private float m_jumpPower = 5f;
    [SerializeField, Min(0f)] private float m_wallJumpPower = 5f;
    [SerializeField, Min(0)] private int m_maxAirJumps = 1;

    [SerializeField] private Vector2 m_floorCheckOffset = Vector2.zero;
    [SerializeField] private Vector2 m_rightWallCheckOffset = Vector2.zero;
    [SerializeField] private Vector2 m_leftWallCheckOffset = Vector2.zero;

    private Rigidbody2D m_rigidBody = null;
    private BoxCollider2D m_collider = null;

    private float m_moveInput = 0f;
    public bool m_isGrounded = true;   // Always assume to be grounded by default
    private bool m_aboutToJump = false;     // Prevent jumping multiple times in single update
    private int m_numAirJumps = 0;
    private int m_lastWallJumpSide = 0;   // 0 none, 1 Right, 2 Left

    private Collider2D m_floorCollider = null;
    private Vector2 m_floorPos = Vector2.zero;

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            m_moveInput = 1f;
        else if (Input.GetKey(KeyCode.A))
            m_moveInput = -1f;
        else
            m_moveInput = 0f;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void FixedUpdate()
    {
        UpdateGroundedState(false);

        Vector2 velocity = m_rigidBody.velocity;

        float maxSpeed = m_maxSpeed;
        if (!m_isGrounded)
            maxSpeed *= m_airSpeedMultiplier;

        if (m_isGrounded && Mathf.Approximately(m_moveInput, 0f))
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, m_brakeFriction * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x += m_moveInput * m_walkAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -m_maxSpeed, m_maxSpeed);
        }

        m_rigidBody.velocity = velocity;

        m_aboutToJump = false;
    }

    public virtual void Jump()
    {
        if (m_aboutToJump)
            return;

        if (!m_isGrounded)
        {
            // Check wall jump first, so we don't consume our mid air jump
            if (m_lastWallJumpSide != 1 && IsAgainstWall(m_rightWallCheckOffset))
            {
                Vector2 velocity = m_rigidBody.velocity;
                velocity = new Vector2(-0.77f, 0.77f) * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_aboutToJump = true;
                m_lastWallJumpSide = 1;
                return;
            }
            else if (m_lastWallJumpSide != 2 && IsAgainstWall(m_leftWallCheckOffset))
            {
                Vector2 velocity = m_rigidBody.velocity;
                velocity = new Vector2(0.77f, 0.77f) * m_wallJumpPower; // Velocity change
                m_rigidBody.velocity = velocity;

                m_aboutToJump = true;
                m_lastWallJumpSide = 2;
                return;
            }
        }

        if (m_isGrounded || m_numAirJumps < m_maxAirJumps)
        {
            Vector2 velocity = m_rigidBody.velocity;
            velocity.y = m_jumpPower; // Velocity change
            m_rigidBody.velocity = velocity;

            if (!m_isGrounded)
                ++m_numAirJumps;

            m_aboutToJump = true;
        }
    }

    private void UpdateGroundedState(bool moveToFloor)
    {
        bool wasGrounded = m_isGrounded;
        if (wasGrounded && moveToFloor)
        {
            // Floor collider might be invalid if destroyed
            if (m_floorCollider)
            {
                Vector2 displacement = (Vector2)m_floorCollider.transform.position - m_floorPos;
                m_rigidBody.position = m_rigidBody.position + displacement;

                Debug.LogFormat("Dif: {0}", displacement.ToString());
            }
        }

        Vector2 groundCheckPos = transform.TransformPoint(m_floorCheckOffset);

        m_floorCollider = Physics2D.OverlapCircle(groundCheckPos, 0.05f);
        if (m_floorCollider != null)
        {
            m_isGrounded = true;
            m_floorPos = m_floorCollider.transform.position;

            if (moveToFloor)
            {
                Vector2 velocity = m_rigidBody.velocity;
                velocity.y = 0f;
                m_rigidBody.velocity = velocity;
            }
            else
            {
                StartCoroutine(PostFixedUpdate());
            }
        }
        else
        {
            m_isGrounded = false;
            m_floorPos = Vector2.zero;
        }

        if (m_isGrounded != wasGrounded)
            if (m_isGrounded)
            {
                m_numAirJumps = 0;
                m_lastWallJumpSide = 0;
            }
    }

    private bool IsAgainstWall(Vector2 offset)
    {
        Vector2 wallCheckPos = transform.TransformPoint(offset);

        Collider2D col = Physics2D.OverlapCircle(wallCheckPos, 0.2f);
        if (col)
            return true;
        else
            return false;
    }

    private IEnumerator PostFixedUpdate()
    {
        yield return new WaitForFixedUpdate();
        UpdateGroundedState(true);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 groundCheckPos = transform.TransformPoint(m_floorCheckOffset);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPos, 0.05f);

        Vector3 rightWallCheckPos = transform.TransformPoint(m_rightWallCheckOffset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(rightWallCheckPos, 0.2f);

        Vector3 leftWallCheckPos = transform.TransformPoint(m_leftWallCheckOffset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(leftWallCheckPos, 0.2f);
    }
}
