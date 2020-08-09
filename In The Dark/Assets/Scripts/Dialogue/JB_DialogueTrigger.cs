using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_DialogueTrigger : MonoBehaviour
{
    public JB_Dialogue dialogue;
    private bool triggered;

    public void TriggerDialogue()
    {
        if (!triggered)
        {
            FindObjectOfType<JB_DialogueManager>().StartDialogue(dialogue);
            triggered = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            TriggerDialogue();
            Debug.Log("testing");
            Destroy(gameObject);

        }

    }
}
