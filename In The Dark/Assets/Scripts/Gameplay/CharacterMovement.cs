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
    [SerializeField] private Rigidbody2D m_rigidBody;               // Rigidbody to move. Used to interact with world
    [SerializeField] private BoxCollider2D m_collider;              // Collider that we move. Used for collision checks

    private float m_horizontalInput = 0f;               // Input for moving horizontally


    private bool m_isGrounded = false;         // If currently grounded   
    private int m_numJumps = 0;                // Amount of jumps while airborne

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

    void Update()
    {
        // For now, handling inputs here
        {
            m_horizontalInput = Input.GetAxis("Horizontal");
        }

        // TODO: Check if we can jump
        if (isGrounded && Input.GetButtonDown("Jump"))
            m_rigidBody.AddForce(new Vector2(0f, m_jumpForce), ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        Vector2 moveVel = new Vector2(m_speed * m_horizontalInput * Time.fixedDeltaTime, 0f);
        m_rigidBody.velocity += moveVel;

        // TODO: OnLanded event
        updateIsGrounded();
    }

    private bool updateIsGrounded()
    {
        m_isGrounded = false;
        if (!m_collider)
            return false;

        float checkSize = 0.1f;

        Vector2 colExtent = m_collider.size * 0.5f;

        // Find area under our 'feet'
        float feetLevel = transform.position.y - colExtent.y;

        Collider2D[] hitCols = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, feetLevel - checkSize),
            new Vector2(colExtent.x * 2f, checkSize * 2f), 0f);
        if (hitCols != null && hitCols.Length > 0)
        {
            foreach (Collider2D col in hitCols)
                if (col.gameObject != gameObject)
                    m_isGrounded = true;
        }

        Debug.Log(m_isGrounded);
        return m_isGrounded;
    }

    #region Debug
    void OnDrawGizmos()
    {
        float checkSize = 0.1f;

        Vector2 colExtent = m_collider.size * 0.5f;

        // Find area under our 'feet'
        float feetLevel = transform.position.y - colExtent.y;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(transform.position.x, feetLevel - checkSize, transform.position.z),
            new Vector3(colExtent.x * 2f, checkSize * 2f, 0.01f));
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterMovement))]
class CharacterMovementEditor : Editor
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
    private void PrintRuntimeValues(CharacterMovement movement)
    {
        EditorGUILayout.Toggle("Is Grounded", movement.isGrounded);
    }
}
#endif