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
    [SerializeField] private Rigidbody2D m_rigidBody;         // Rigidbody to move. Used to interact with world
    [SerializeField] private Collider2D m_collider;         // Collider that we move, ideally a capsule or box

    void Awake()
    {
        if (!m_rigidBody)
            m_rigidBody = GetComponent<Rigidbody2D>();

        //if (m_rigidBody)
            //m_rigidBody.isKinematic = true;
    }

    void FixedUpdate()
    {
        float moveXInput = Input.GetAxis("Horizontal");
        m_rigidBody.velocity = new Vector2((moveXInput * 10f), m_rigidBody.velocity.y);

        if (Input.GetButtonDown("Jump"))
            m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, 14f);

        m_rigidBody.velocity += Physics2D.gravity * Time.fixedDeltaTime;


    }
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
        
    }
}
#endif