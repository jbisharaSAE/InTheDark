using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordTrigger : MonoBehaviour
{
    [SerializeField] private JB_ResourceManagement resourceScript;
    public bool bAttack;
    public bool bThirdattack;



    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        resourceScript.BasicSwordAttack();
    //        // did the sword hit an enemy
    //    }
    //    else if (collision.gameObject.tag == "Boss")
    //    {
    //        // did the sword hit a boss
    //    }
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("testing collision stay");
        // did the sword hit an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            
            // are we hitting with the third attack or first two
            if (bAttack)
            {
                bAttack = false;
                // take enemy hp
                // 2 for third attack
                DamageEnemy(1, collision.gameObject);

            }
            else if (bThirdattack)
            {

                bThirdattack = false;
                RandomGeneratedCombo();
                // take enemy hp
                // 1 for non third attack
                DamageEnemy(2, collision.gameObject);
            }

            
        }
        else if (collision.gameObject.tag == "Boss")
        {
            // did the sword hit a boss
        }
    }

    private void RandomGeneratedCombo()
    {
        int rand = Random.Range(1, 4);
        resourceScript.BasicSwordAttack(rand);
    }

    private void DamageEnemy(int attackPhase, GameObject enemy)
    {
        switch (attackPhase)
        {
            case 1:
                enemy.GetComponent<HealthComponent>().ApplyDamage(10f);
                break;
            case 2:
                enemy.GetComponent<HealthComponent>().ApplyDamage(25f);
                break;
        }
    }
}
