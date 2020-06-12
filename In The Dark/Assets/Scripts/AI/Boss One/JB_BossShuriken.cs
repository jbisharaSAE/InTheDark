using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossShuriken : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 25f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.right * speed;
    }
}
