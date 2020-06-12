using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossOne : MonoBehaviour
{
    private Transform player;
    private Animator anim;

    public GameObject shurikenPrefab;
    public Transform shurikenSpawn;
    public bool isFlipped = false;
    public float moveSpeed;

    private float throwTimer = 7f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(throwTimer >= 0)
        {
            throwTimer -= Time.deltaTime;
        }
    }
    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void ThrowShuriken()
    {
        
        float rand = Random.value;
        if(throwTimer <= 0f)
        {
            
            if (rand < 0.4f)
            {
                // throw shuriken
                anim.SetTrigger("Throw");
                GameObject shuriken = Instantiate(shurikenPrefab, shurikenSpawn.position, transform.rotation);
                
            }
            throwTimer = 7f;
        }
        
    }
}
