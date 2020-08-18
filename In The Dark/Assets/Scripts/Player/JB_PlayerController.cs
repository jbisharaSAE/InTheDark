using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

//This Script is intended for demoing and testing animations only.


public class JB_PlayerController : MonoBehaviour
{

    public Transform throwSpawn;
    public GameObject kunaiPrefab;
    public GameObject parryColliderObj;
    public Image dashBar;
    private GameObject kunaiShuriken;
    private Animator anim;
    
    //movement + attack 
    private float hSpeed = 10f;
	private float moveXInput;
    private float attackTimer = 0.0f;
    private float dashRecharge = 100.0f; 
    public float dashRefillSpeed;
    public int dashCharge = 2;
    public int attackPhase = 0;
    private bool m_isFacingRight = true;
    private bool m_isAttacking = false;
    private Rigidbody2D m_rigidBody;
    private JB_ResourceManagement resourceScript;
    private AdvancedCharacterMovement advancedScript;
    private AudioSource audioSource;


    [SerializeField] private JB_SwordTrigger swordScript;

    [Header("Audio: SFX")]
    [SerializeField] private AudioClip[] swordSwings;
    [SerializeField] private AudioClip shurikenThrowSFX;
    [SerializeField] private AudioClip swordSlashSFX;
    [SerializeField] private AudioClip aoeSlash;
    [SerializeField] private AudioClip healSFX;

    //Used for flipping Character Direction
    public static Vector3 playerScale;

    public bool isFacingRight { get { return m_isFacingRight; } }
    public bool isAttacking { get { return m_isAttacking; } set { m_isAttacking = value} }

    // Use this for initialization
    void Awake ()
	{
        m_rigidBody = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator> ();

        resourceScript = GetComponent<JB_ResourceManagement>();

        advancedScript = GetComponent<AdvancedCharacterMovement>();

        audioSource = GetComponent<AudioSource>();
    }

	
	void Update()
	{
        #region player_input
        UpdateInput();
        #endregion

        anim.SetBool("ground", advancedScript.isGrounded);

        if (dashRecharge <= 100.0f)
        {
            dashRecharge += Time.deltaTime * dashRefillSpeed;
        }

        if (dashBar)
            dashBar.fillAmount = dashRecharge / 100.0f;

        if(dashRecharge < 50.0f)
        {
            dashCharge = 0;
        }
        else if(dashRecharge >= 50.0f && dashRecharge < 100.0f)
        {
            dashCharge = 1;
        }
        else if(dashRecharge == 100.0f)
        {
            dashCharge = 2;
        }

        



    }

    void UpdateInput()
    {
        if (GameManager.isInputDisabled)
        {
            advancedScript.SetMoveInput(0f);
            moveXInput = 0f;
            return;
        }

        moveXInput = Input.GetAxisRaw("Horizontal");
        advancedScript.SetMoveInput(moveXInput);

        if (Input.GetButtonDown("Jump"))
        {
            //anim.SetBool("ground", false);

            //GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.y, jumpForce);
            advancedScript.Jump();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            advancedScript.StopJumping();
        }


        anim.SetFloat("HSpeed", Mathf.Abs(moveXInput));
        anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);


        //m_rigidBody.velocity = new Vector2((moveXInput * hSpeed), m_rigidBody.velocity.y);

        // left mouse button - attack
        if (Input.GetButtonDown("Fire1"))
        {

            Attack();

        }

        // used to measure attack animation with mouse click
        if (attackTimer >= 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackPhase = 0;
            m_isAttacking = false;
            //resourceScript.attackPhase = attackPhase;
        }


        // right mouse button - shuriken throw
        if (Input.GetButtonDown("Fire2"))
        {

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
        if (moveXInput > 0 && !m_isFacingRight)
        {
            Flip();
            //dir = new Vector2(1f, 0f);

        }
        else if (moveXInput < 0 && m_isFacingRight)
        {
            Flip();
            //dir = new Vector2(-1f, 0f);

        }


        // raycasting - 11 layer should be environment / ground
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, 11);

        // dash ability
        if (Input.GetKeyDown(KeyCode.E))
        {
            Dash();
        }

        //Debug.DrawRay(transform.position, dir*10f, Color.green);

        // player abilities
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // play sword slash sfx
            audioSource.PlayOneShot(swordSlashSFX);

            resourceScript.PlayerAbilities(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // play aoe slash sfx
            //audioSource.PlayOneShot(aoeSlash);

            resourceScript.PlayerAbilities(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // play heal sfx
            audioSource.PlayOneShot(healSFX);

            resourceScript.PlayerAbilities(3);
        }

        // parry ability
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ParryCollider());
        }
    }

    IEnumerator ParryCollider()
    {
        parryColliderObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        parryColliderObj.SetActive(false);
    }

    private void Dash()
    {
        

        if (dashCharge == 2 )
        {
            --dashCharge;
            dashRecharge = 50.0f;
            advancedScript.Dash();
        }
        else if(dashCharge == 1)
        {
            --dashCharge;
            dashRecharge = 0.0f;
            advancedScript.Dash();
        }

       
    }

    //Flipping direction of character
    void Flip()
	{
		m_isFacingRight = !m_isFacingRight;
		playerScale = transform.localScale;
		playerScale.x *= -1;
		transform.localScale = playerScale;
	}

    private void Attack()
    {
        if(resourceScript.currentEnergy > 10.0f)
        {
            m_isAttacking = true;
            attackTimer = 1.0f;

            switch (attackPhase)
            {
                case 0:
                    resourceScript.UpdateEnergy(-10.0f);

                    // play sound
                    audioSource.PlayOneShot(swordSwings[0]);

                    swordScript.PlayerAttack(1);
                    
                    // attack animation 1
                    anim.SetTrigger("attackOne");
                    break;
                case 1:
                    resourceScript.UpdateEnergy(-20.0f);

                    // play sound
                    audioSource.PlayOneShot(swordSwings[1]);

                    swordScript.PlayerAttack(1);
                    
                    // attack animation 2
                    anim.SetTrigger("attackTwo");
                    break;
                case 2:
                    
                    resourceScript.UpdateEnergy(-25.0f);

                    // play sound
                    audioSource.PlayOneShot(swordSwings[2]);

                    swordScript.PlayerAttack(2);

                    // attack animation 3
                    anim.SetTrigger("stab");
                    
                    break;

            }

            ++attackPhase;
            //resourceScript.attackPhase = attackPhase;
        }
        

        if (attackPhase > 2)
        {
            attackPhase = 0;
            //resourceScript.attackPhase = attackPhase;
        }
        
    }

  
    private void ThrowShuriken()
    {
        // throwing a shuriken requires a combo point, we are checking to see if we any to use
        if(resourceScript.currentCombo > 0)
        {
            anim.SetTrigger("Throw");

            // play shuriken throw sound
            audioSource.PlayOneShot(shurikenThrowSFX);

            // calls function to adjust combo point
            resourceScript.UpdateComboPoints(-1);

            // throwing shuriken in right direction
            //if (m_isFacingRight)
            //{
            //    kunaiShuriken = Instantiate(kunaiPrefab, throwSpawn.position, kunaiPrefab.transform.rotation);
            //    kunaiShuriken.GetComponent<JB_Shuriken>().facingRight = m_isFacingRight;


            //    Vector3 newScale = kunaiShuriken.transform.localScale;
            //    newScale.y *= 1;

            //    kunaiShuriken.GetComponent<Transform>().transform.localScale = newScale;
            //}
            //// throwing shuriken in left direction
            //else
            //{

            //    kunaiShuriken = Instantiate(kunaiPrefab, throwSpawn.position, kunaiPrefab.transform.rotation);
            //    kunaiShuriken.GetComponent<JB_Shuriken>().facingRight = m_isFacingRight;

            //    Vector3 newScale = kunaiShuriken.transform.localScale;
            //    newScale.y *= -1;

            //    kunaiShuriken.GetComponent<Transform>().transform.localScale = newScale;
            //}

            
        }
        
        
    }

   

}
