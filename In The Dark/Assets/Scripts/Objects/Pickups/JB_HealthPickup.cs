using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_HealthPickup : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;

    [SerializeField] private float power;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();

        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-10f, 10f);

        Vector2 dir = new Vector2(randX, randY);
        dir.Normalize();
        m_RigidBody.AddForce((dir * power));
    }

    
}
