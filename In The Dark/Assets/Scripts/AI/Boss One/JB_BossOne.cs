using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossOne : MonoBehaviour
{
    private Transform player;
    private Transform playerTargetLocation;
    private Animator anim;
    private Rigidbody2D rb;
    

    public GameObject smokeBombPrefab;
    public GameObject shurikenPrefab;
    public Transform shurikenSpawn;
    public bool isFlipped = false;
    public float moveSpeed;
    public float fallSpeed;
    public float bossVanishHeight = 100f;
    private float throwTimer = 7f;
    public bool bVanished = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("BossVanish", 10f, 10f);
    }

    private void Update()
    {
        if(throwTimer >= 0)
        {
            throwTimer -= Time.deltaTime;
        }

        if (bVanished)
        {
            
            //transform.position = new Vector2(player.transform.position.x, transform.position.y);

        }
        else
        {
            
            

            
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

    public void BossVanish()
    {
        float rand = Random.value;

        if(rand < 0.25f)
        {
            Debug.Log("testing random number generator");
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + 2.52f);
            Instantiate(smokeBombPrefab, newPos, smokeBombPrefab.transform.rotation);
            transform.position = new Vector3(transform.position.x, transform.position.y + 60f, transform.position.z);
            bVanished = true;
            StartCoroutine(FindPlayerToLandOn());
            //anim.SetTrigger("Vanish");
        }

        anim.ResetTrigger("Vanish");
    }

    private IEnumerator FindPlayerToLandOn()
    {
        yield return new WaitForSeconds(2f);

        // setting boss x position to player x position
        transform.position = new Vector2(player.transform.position.x, transform.position.y);
        playerTargetLocation = player.transform;

        // finding distance between boss and player
        float distance = Vector2.Distance(rb.position, playerTargetLocation.position);

        while (distance > 0.1f)
        {
            distance = Vector2.Distance(rb.position, playerTargetLocation.position);
            transform.position = Vector2.MoveTowards(rb.position, playerTargetLocation.transform.position, fallSpeed * Time.deltaTime);
            //rb.MovePosition(newPos);
            yield return null;
        }
        
        

        
        bVanished = false;
        anim.SetBool("IsVanished", false);
    }
}
