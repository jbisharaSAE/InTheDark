using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class JB_Dialogue : ScriptableObject
{
    public dialogueData[] interactions;
}


[System.Serializable]
public struct dialogueData
{
    public Sprite characterSprite;
    public string name;
    [TextArea(3, 10)]
    public string sentences;
}

