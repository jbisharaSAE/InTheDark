using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            bossPrefab.SetActive(true);

            GameManager.SetInputDisabled(true);
            //GameManager.inputDisabled;
        }
    }

}
