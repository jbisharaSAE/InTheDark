using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossShuriken : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 25f;
    [SerializeField] private DamageInfo shurikenDmg;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthComponent>().ApplyDamage(shurikenDmg);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
