using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordAttack : MonoBehaviour
{
    private enum AttackType { SwordSlash, AoeSlash };

    [SerializeField] private AttackType m_attackType;
    [SerializeField] private float speed = 25f;
    [SerializeField] private DamageInfo m_dmgAmount;

    private Rigidbody2D m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        if (m_attackType == AttackType.SwordSlash)
            m_rigidBody = GetComponent<Rigidbody2D>();

        if(m_attackType == AttackType.AoeSlash)
        {
            Destroy(gameObject, 0.2f);
        }
        else
        {
            Destroy(gameObject, 5f);
        }
    }

    private void Update()
    {
        if (m_attackType == AttackType.SwordSlash)
            m_rigidBody.velocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<HealthComponent>() != null)
        {
            Debug.Log("testing health component");
            col.gameObject.GetComponent<HealthComponent>().ApplyDamage(m_dmgAmount, new DamageEvent(gameObject, transform.root.position));
            Destroy(gameObject);
        }
    }

    
}
