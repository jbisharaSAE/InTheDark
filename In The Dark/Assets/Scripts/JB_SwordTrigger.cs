using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordTrigger : MonoBehaviour
{
    public JB_ResourceManagement resourceScript;
    //public bool bThirdattack;

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
        if (collision.gameObject.tag == "Enemy")
        {
            // did the sword hit an enemy
            RandomGeneratedCombo();
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
}
