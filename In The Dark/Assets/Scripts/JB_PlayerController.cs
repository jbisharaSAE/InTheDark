using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

//This Script is intended for demoing and testing animations only.


public class JB_PlayerController : MonoBehaviour {

    public Transform throwSpawn;
    public GameObject kunaiPrefab;
    private GameObject kunaiShuriken;
    private Animator anim;
    
    //movement + attack 
    private float hSpeed = 10f;
	private float moveXInput;
    private float attackTimer = 0.0f;
    public float dashForce;
    public int attackPhase = 0;
    private bool bFacingRight = true;
    private Rigidbody2D rb;
    private bool bDashing;
    private BoxCollider2D playerBoxCollider;
    private Vector2 dir;
    public GameObject sword;
    private JB_ResourceManagement resourceScript;


    //Used for flipping Character Direction
	public static Vector3 playerScale;

	//jumping 
	public Transform groundCheck;
	public LayerMask whatIsGround;
	private bool bGrounded = false;
	private float groundRadius = 0.15f;
	private float jumpForce = 14f;

	
    

	// Use this for initialization
	void Awake ()
	{
        rb = GetComponent<Rigidbody2D>();

        playerBoxCollider = GetComponent<BoxCollider2D>();

        anim = GetComponent<Animator> ();

        resourceScript = GetComponent<JB_ResourceManagement>();

    }

	void FixedUpdate ()
	{

		bGrounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool ("ground", bGrounded);


	}

	void Update()
	{
       #region player_input

        moveXInput = Input.GetAxis("Horizontal");

        if ((bGrounded) && Input.GetButtonDown("Jump"))
        {
            anim.SetBool("ground", false);

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.y, jumpForce);
        }


        anim.SetFloat("HSpeed", Mathf.Abs(moveXInput));
        anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);


        rb.velocity = new Vector2((moveXInput * hSpeed), rb.velocity.y);

        // left mouse button - attack
        if (Input.GetButtonDown("Fire1"))
        {
            
            Attack();
            
        }

        // used to measure attack animation with mouse click
        if(attackTimer >= 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackPhase = 0;
            resourceScript.attackPhase = attackPhase;
        }
      

        // right mouse button - shuriken throw
        if(Input.GetButtonDown("Fire2"))
        {
            anim.SetTrigger("Throw");

            ThrowShuriken();

        }

        // increases character speed * sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("Sprint", true);
            hSpeed = 25f;
}
        else
        {
            anim.SetBool("Sprint", false);
            hSpeed = 10f;
        }

        //Flipping direction character is facing based on players Input
        if (moveXInput > 0 && !bFacingRight)
        {
            Flip();
            dir = new Vector2(1f, 0f);

        }
        else if (moveXInput < 0 && bFacingRight)
        {
            Flip();
            dir = new Vector2(-1f, 0f);

        }


        // raycasting - 11 layer should be environment / ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, 11);

        // dash ability
        if (Input.GetKeyDown(KeyCode.E))
        {
            StopAllCoroutines();
            Dash(bFacingRight, hit);
            Debug.Log("e button pushed");
        }

        //Debug.DrawRay(transform.position, dir*10f, Color.green);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            resourceScript.PlayerAbilities(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            resourceScript.PlayerAbilities(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            resourceScript.PlayerAbilities(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            resourceScript.PlayerAbilities(4 );
        }
        #endregion
    }

    private void Dash(bool rightDir, RaycastHit2D hit)
    {
        bDashing = true;
        float dashDistance = 5f;
        StartCoroutine(PlayDashAnim());

        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.x - transform.position.x);
            if (distance < 5f)
            {
                dashDistance = distance;
            }
        }

        if (rightDir)
        {
            
            transform.position += Vector3.right * dashDistance;
        }
        else
        {
            transform.position += Vector3.left * dashDistance;
        }
    }

    IEnumerator PlayDashAnim()
    {
        // used to temporarily ignore enemy collisions while dashing
        yield return new WaitForSeconds(0.5f);
        // reapplies collision between enemy and player
        bDashing = false;
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    //Flipping direction of character
    void Flip()
	{
		bFacingRight = !bFacingRight;
		playerScale = transform.localScale;
		playerScale.x *= -1;
		transform.localScale = playerScale;
	}

    private void Attack()
    {
        if(resourceScript.currentEnergy > 10.0f)
        {
            attackTimer = 1.0f;

            

            switch (attackPhase)
            {
                case 0:
                    resourceScript.currentEnergy -= 10.0f;
                    Debug.Log(attackPhase + " case 0");
                    // attack animation 1
                    break;
                case 1:
                    resourceScript.currentEnergy -= 20.0f;
                    Debug.Log(attackPhase + " case 1");
                    
                    // attack animation 2
                    break;
                case 2:
                    resourceScript.currentEnergy -= 25.0f;
                    //sword.GetComponent<JB_SwordTrigger>().bThirdattack = true;
                    resourceScript.bThirdattack = true;
                    Debug.Log(attackPhase + " case 2");
                    
                    //UpdateComboPoints();
                    // attack animation 3
                    break;

            }

            ++attackPhase;
            resourceScript.attackPhase = attackPhase;
        }
        

        if (attackPhase > 2)
        {
            attackPhase = 0;
            resourceScript.attackPhase = attackPhase;
        }
        anim.SetTrigger("Punch");
    }

   

    private void ThrowShuriken()
    {
        // throwing shuriken in right direction
        if (bFacingRight)
        {
            kunaiShuriken = Instantiate(kunaiPrefab, throwSpawn.position, kunaiPrefab.transform.rotation);
            kunaiShuriken.GetComponent<JB_Kunai>().facingRight = bFacingRight;


            Vector3 newScale = kunaiShuriken.transform.localScale;
            newScale.y *= 1;

            kunaiShuriken.GetComponent<Transform>().transform.localScale = newScale;
        }
        // throwing shuriken in left direction
        else
        {
            
            kunaiShuriken = Instantiate(kunaiPrefab, throwSpawn.position, kunaiPrefab.transform.rotation);
            kunaiShuriken.GetComponent<JB_Kunai>().facingRight = bFacingRight;

            Vector3 newScale = kunaiShuriken.transform.localScale;
            newScale.y *= -1;

            kunaiShuriken.GetComponent<Transform>().transform.localScale = newScale; 
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if player hits enemy while dashing ignore collision
        if (bDashing && collision.gameObject.tag == "Enemy")
        {
            Debug.Log("testing enemy collision");
            // layer 10 is player, layer 8 is enemy
            Physics2D.IgnoreLayerCollision(10, 8);
        }
        // if player hits boss while dashing ignore collision
        if (bDashing && collision.gameObject.tag == "Boss")
        {
            Physics2D.IgnoreCollision(playerBoxCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if player hits enemy while dashing ignore collision
        if (bDashing && collision.gameObject.tag == "Enemy")
        {
            Debug.Log("testing enemy collision");
            // layer 10 is player, layer 8 is enemy
            Physics2D.IgnoreLayerCollision(10, 8);
        }
        // if player hits boss while dashing ignore collision
        if (bDashing && collision.gameObject.tag == "Boss")
        {
            Physics2D.IgnoreCollision(playerBoxCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }

}
