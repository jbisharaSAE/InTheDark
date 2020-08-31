using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostDialogueFinishLevel : MonoBehaviour
{
    [SerializeField] private JB_Dialogue m_dialogueToRespondTo = null;

    void Start()
    {
        JB_DialogueManager.onDialogueFinished += OnDialogueFinished;
    }

    void OnDestroy()
    {
        JB_DialogueManager.onDialogueFinished -= OnDialogueFinished;
    }

    private void OnDialogueFinished(JB_Dialogue dialogue)
    {
        if (dialogue == null)
            return;

        if (dialogue == m_dialogueToRespondTo)
            GameManager.FinishCampaignLevel();
    }
}
