using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This component handles the movement of a character. It does not directly move the character
/// but rather has inputs that can be used to do so
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float m_speed = 10f;             // Speed of horizontal movement
    public int m_jumps = 1;                 // Max amount of airborne jumps allowed
    public float m_jumpForce = 10f;         // Force of jumps

    [Header("Components")]
    [SerializeField] protected Rigidbody2D m_rigidBody;                 // Rigidbody to move. Used to interact with world
    [SerializeField] protected BoxCollider2D m_collider;                // Collider that we move. Used for collision checks

    [Header("Config")]
    [SerializeField] protected LayerMask m_worldLayers = Physics2D.AllLayers;         // Layer Mask for world collision checks

    private float m_horizontalInput = 0f;               // Input for moving horizontally


    protected bool m_isGrounded = false;                // If currently grounded   

    private Collider2D m_floor;                             // Floor character is standing on
    private Vector2 m_floorLocation = Vector2.zero;         // Location floor was when last updated

    /// <summary>
    /// If character is currently grounded
    /// </summary>
    public bool isGrounded { get { return m_isGrounded; } }

    void Awake()
    {
        if (!m_rigidBody)
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            if (!m_rigidBody)
                enabled = false;
        }

        if (m_rigidBody)
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!m_collider)
            m_collider = GetComponent<BoxCollider2D>();
    }

    protected virtual void FixedUpdate()
    {
        // TODO: This handles slow startup when moving on ground, but avoids
        // removing all horizontal movement when in air (ideally we shouldn't need to if check)
        // (velocity.y check is for checking if we are about to jump)
        if (m_isGrounded && m_rigidBody.velocity.y <= 0f)
            m_rigidBody.velocity = new Vector2(m_horizontalInput * m_speed, 0f);
        else
            m_rigidBody.velocity += new Vector2(m_horizontalInput * m_speed * Time.fixedDeltaTime, 0f);

        // TODO: OnLanded event
        UpdateIsGrounded();

        // Fake Friction
        if (Mathf.Approximately(m_horizontalInput, 0f) && m_isGrounded)
            m_rigidBody.velocity -= m_rigidBody.velocity * 0.1f;
    }

    /// <summary>
    /// Set the horizontal input of movement
    /// </summary>
    /// <param name="input">amount of input to apply</param>
    public virtual void SetHorizontalInput(float input)
    {
        m_horizontalInput = input;
    }

    /// <summary>
    /// Have character jump once if able to.
    /// </summary>
    /// <returns>If a jump was performed</returns>
    public virtual bool Jump()
    {
        if (m_isGrounded)
        {
            //m_rigidBody.AddForce(new Vector2(0f, m_jumpForce), ForceMode2D.Impulse);
            m_rigidBody.velocity += new Vector2(0f, m_jumpForce);
            return true;
        }

        return false;
    }

    private bool UpdateIsGrounded()
    {
        m_isGrounded = false;
        if (!m_collider)
            return false;

        Collider2D prevFloor = m_floor;
        m_floor = null;

        float checkSize = 0.1f;

        Vector2 colExtent = m_collider.size * 0.5f;

        // Find area under our 'feet'
        float feetLevel = transform.position.y - colExtent.y;

        Collider2D[] hitCols = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, feetLevel - checkSize),
            new Vector2(colExtent.x * 2f * 0.9f, checkSize * 2f), 0f);
        if (hitCols != null && hitCols.Length > 0)
        {
            foreach (Collider2D col in hitCols)
                if (col.gameObject != gameObject)
                {
                    m_isGrounded = true;
                    m_floor = col;
                    break;
                }
        }

        if (m_floor)
        {
            if (m_floor == prevFloor)
            {
                // This is fine in the meantime for horizontal moving platforms. We might
                // have to ultimately implement our own movement logic and collision checks for best results
                Vector2 diff = (Vector2)m_floor.transform.position - m_floorLocation;
                if (diff.sqrMagnitude > 0f)
                {
                    m_rigidBody.position += diff;
                }
            }

            m_floorLocation = m_floor.transform.position;
        }
            

        return m_isGrounded;
    }

    #region Debug
    protected virtual void OnDrawGizmos()
    {
        float checkSize = 0.1f;

        Vector2 colExtent = m_collider.size * 0.5f;

        // Find area under our 'feet'
        float feetLevel = transform.position.y - colExtent.y;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, feetLevel - checkSize, transform.position.z),
            new Vector3(colExtent.x * 2f * 0.9f, checkSize * 2f, 0.01f));
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterMovement))]
public class CharacterMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        PrintRuntimeValuesLabel();
        PrintRuntimeValues(serializedObject.targetObject as CharacterMovement);
    }
    
    /// <summary>
    /// This function simply prints the header for the runtime values section
    /// </summary>
    private void PrintRuntimeValuesLabel()
    {
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Runtime Values");
        EditorStyles.label.fontStyle = FontStyle.Normal;
    }

    /// <summary>
    /// This function prints label of all the runtime values
    /// </summary>
    /// <param name="movement">Movement component to print</param>
    protected virtual void PrintRuntimeValues(CharacterMovement movement)
    {
        EditorGUILayout.Toggle("Is Grounded", movement.isGrounded);
    }
}
#endif