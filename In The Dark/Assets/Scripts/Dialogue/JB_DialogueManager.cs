using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JB_DialogueManager : MonoBehaviour
{
    //public Animator animator;
    public GameObject systemPanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Image characterImage;
    

    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<Sprite> characterImg;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        names = new Queue<string>();
        characterImg = new Queue<Sprite>();
    }

    public void StartDialogue(JB_Dialogue dialogue)
    {
        //animator.SetBool("IsOpen", true);
        systemPanel.SetActive(true);
        //Time.timeScale = 0.0f;

        // clearing current queues to make sure we dont repeat previous dialogues
        names.Clear();
        sentences.Clear();
        characterImg.Clear();

        // initialising values from dialogue class into queue variable

        for (int i = 0; i < dialogue.interactions.Length; ++i)
        {
            names.Enqueue(dialogue.interactions[i].name);
            sentences.Enqueue(dialogue.interactions[i].sentences);
            characterImg.Enqueue(dialogue.interactions[i].characterSprite);
        }


        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // end dialogue
        if (sentences.Count < 1)
        {
            EndDialogue();

            return;

        }

        string sentence = sentences.Dequeue();
        string name = names.Dequeue();
        Sprite character = characterImg.Dequeue();

        nameText.text = name;
        characterImage.sprite = character;
        //DisplayCharacterSprite(name);

        StopAllCoroutines();
        StartCoroutine(CoTypeSentence(sentence));

    }

  

    IEnumerator CoTypeSentence(string sentence)
    {
        yield return new WaitForSeconds(0.2f);
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        //animator.SetBool("IsOpen", false);
        systemPanel.SetActive(false);
        GameManager.SetInputDisabled(false);
        //Time.timeScale = 1f;
    }
}