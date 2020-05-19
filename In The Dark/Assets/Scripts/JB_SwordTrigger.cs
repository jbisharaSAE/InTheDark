using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordTrigger : MonoBehaviour
{
    public JB_EnergyResourceManagement resourceScript;
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
            Debug.Log("testing collision trigger");
            resourceScript.BasicSwordAttack();
            // did the sword hit an enemy
        }
        else if (collision.gameObject.tag == "Boss")
        {
            // did the sword hit a boss
        }
    }
}
