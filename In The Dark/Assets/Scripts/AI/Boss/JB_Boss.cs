﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Boss : MonoBehaviour
{
    private enum BossType { bossOne, bossTwo, bossThree };

    private Transform player;
    private Vector2 playerTargetLocation;
    private Animator anim;
    private Rigidbody2D rb;
    private float throwTimer = 7f;
    private bool isFlipped = true;
    private PatrolArea addPatrolArea;

    [SerializeField] private BossType bossType;
    [SerializeField] private GameObject bossBombPrefab;
    [SerializeField] private GameObject bossAddPrefab;
    [SerializeField] private GameObject bossShieldPrefab;
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private Transform shurikenSpawn;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float bossVanishHeight = 100f;

    public float moveSpeed { get { return m_moveSpeed; } }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        addPatrolArea = GetComponent<PatrolArea>();

        switch (bossType)
        {
            case BossType.bossOne:
                InvokeRepeating("BossVanish", 10f, 10f);
                break;
            case BossType.bossTwo:
                InvokeRepeating("BossSummon", 10f, 10f);
                break;
            case BossType.bossThree:
                break;
        }
        
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

    private IEnumerator FindPlayerToLandOn(GameObject obj)
    {
        yield return new WaitForSeconds(2f);

        // setting boss x position to player x position
        obj.transform.position = new Vector2(player.transform.position.x, transform.position.y);
        playerTargetLocation = new Vector2(obj.transform.position.x, player.transform.position.y);

        // finding distance between boss and player
        float distance = Vector2.Distance(obj.transform.position, playerTargetLocation);
        anim.SetBool("IsVanished", false);
        Debug.Log("Testing While Loop");

        // move boss to the player location
        while (distance > 0.1f)
        {
            distance = Vector2.Distance(obj.transform.position, playerTargetLocation);
            transform.position = Vector2.MoveTowards(obj.transform.position, playerTargetLocation, fallSpeed * Time.deltaTime);
            //rb.MovePosition(newPos);
            yield return null;
        }
        gameObject.GetComponent<JB_BossAttack>().BossAttack();

    }

    #region boss_one

    private void BossVanish()
    {
        float rand = Random.value;

        if(rand < 0.25f)
        {
            
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + 2.52f);
            Instantiate(bossBombPrefab, newPos, bossBombPrefab.transform.rotation);
            transform.position = new Vector3(transform.position.x, transform.position.y + bossVanishHeight, transform.position.z);
            anim.SetBool("IsVanished", true);
            StartCoroutine(FindPlayerToLandOn(gameObject));
            
        }

        anim.ResetTrigger("Vanish");
    }




    #endregion


    #region boss_two
    private void BossSummon()
    {
        float rand = Random.value;

        if (rand < 1f)
        {
            // get player location
            // summmon add above player location
            Vector2 spawnAddLocation = new Vector3(player.transform.position.x, bossVanishHeight);
            GameObject summonedAdd = Instantiate(bossAddPrefab, spawnAddLocation, bossAddPrefab.transform.rotation);
            summonedAdd.GetComponent<EnemyTargetSelector>().overrideTarget = player.transform.gameObject;
            summonedAdd.GetComponent<BruteEnemyScript>().patrolArea = addPatrolArea;
            StartCoroutine(FindPlayerToLandOn(summonedAdd));

            // add falls on top of player

        }
    }
    #endregion

    #region boss_three
    private void BossShield()
    {
        float rand = Random.value;

        if(rand < 0.25f)
        {
            // spawn a reflective shield  that needs to be destroyed before attacking boss
        }
    }
    #endregion

}
