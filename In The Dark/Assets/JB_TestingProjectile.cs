using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_TestingProjectile : MonoBehaviour
{
    public float speed = 25f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.left * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
