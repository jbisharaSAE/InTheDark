using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAlarm : MonoBehaviour
{
    public GameObject routeHolder;
    public GameObject[] routes;
    public GameObject player;
    public bool trigger;
    public bool test;

    private void Start()
    {
        trigger = false;
        player = GameObject.FindWithTag("Player"); 
    }

    private void Update(){
        if (test)
        {
            test = false;
            Alarm();
        }
    }
   private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !trigger)
        {
            trigger = true;
            Alarm();
        }
    }

   public void Alarm()
   {
        foreach(GameObject i in routes)
        {
            i.GetComponent<Transform>().SetPositionAndRotation(routeHolder.transform.position, this.transform.rotation);
        }
   }
}
