using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_HealthPickup : MonoBehaviour
{
    private Rigidbody2D m_RigidBody;
    public float speed;

    [SerializeField] private float power;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();

        float randX = Random.Range(-5f, 5f);
        float randY = Random.Range(-5f, 5f);

        //Vector2 dir = new Vector2(randX, randY);

        Vector2 dir = Random.insideUnitSphere.normalized;
        //dir.Normalize();
        m_RigidBody.AddForce((dir * power));
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward *speed * Time.deltaTime);
    }

}
