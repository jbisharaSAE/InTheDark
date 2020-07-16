using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossOneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject bossOne;
    

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            bossOne.SetActive(true);

            GameManager.SetInputDisabled(true);
            //GameManager.inputDisabled;
        }
    }

}
