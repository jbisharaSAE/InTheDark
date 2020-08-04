using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordAttack : MonoBehaviour
{
    private enum AttackType { SwordSlash, AoeSlash };

    [SerializeField] private AttackType m_attackType;
    [SerializeField] private float speed = 25f;
    

    private Rigidbody2D m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (m_attackType == AttackType.SwordSlash)
            m_rigidBody.velocity = transform.right * speed;
    }

}
