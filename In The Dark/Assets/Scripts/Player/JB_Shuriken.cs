﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Shuriken : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 100f;
    public float direction = -1.0f;
    public bool facingRight = true;
    public DamageInfo shurikenDamage;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("spawned");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (facingRight)
        {
            rb.velocity = (Vector2.right) * speed;
        }
        else
        {
            rb.velocity = (Vector2.left) * speed;
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<HealthComponent>())
        {
            collision.gameObject.GetComponent<HealthComponent>().ApplyDamage(shurikenDamage);
        }
        Destroy(gameObject);
    }
}
