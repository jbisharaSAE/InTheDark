using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAmbush : MonoBehaviour
{
    public GameObject[] badGuys;
    public GameObject player;
    public bool trigger;
    public bool test;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (test)
        {
            test = false;
            Ambush();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !trigger)
        {
            trigger = true;
            Ambush();
        }
    }

    public void Ambush()
    {
        foreach (GameObject i in badGuys)
        {
            i.SetActive(true);
        }
    }

    // need to add trigger for beating both bad guys to open door to leave room
}
