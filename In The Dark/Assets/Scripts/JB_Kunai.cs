using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Kunai : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
