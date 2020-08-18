using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAmbush : MonoBehaviour
{
    public GameObject[] badGuys;
    public GameObject player;
    public GameObject door;
    public GameObject doorOpen;
    public GameObject doorClosed;
    public bool trigger;
    public bool test;

    private bool moveDoor;
    private Vector3 doorNewPos;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        moveDoor = false;
    }

    void Update()
    {
        if (test)
        {
            test = false;
            Ambush();
        }

        if (badGuys[0] == null && badGuys[1] == null)
        {
            OpenDoor();
        }

        if (moveDoor == true)
        {
            door.transform.SetPositionAndRotation(doorNewPos * Time.deltaTime, doorClosed.transform.rotation);
            if (door.transform.position == doorNewPos)
            {
                moveDoor = false;
            }
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
        doorNewPos = doorClosed.transform.position;
        moveDoor = true;
        foreach (GameObject i in badGuys)
        {
            i.SetActive(true);
        }

    }

    public void OpenDoor()
    {
        doorNewPos = doorOpen.transform.position;
        moveDoor = true;
    }
}