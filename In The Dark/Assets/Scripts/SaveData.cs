using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class NinjasSaveData
{
    // The levels the player had unlocked, should at least be one
    [SerializeField]
    public List<string> m_unlockedLevels = new List<string>();

    // The level the player is up to or last played
    [SerializeField]
    public string m_currentLevel = string.Empty;

    public void Save()
    {
        string fullPath = GetFullPath();
        FileStream stream = new FileStream(fullPath, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, this);

        stream.Close();
    }

    public static NinjasSaveData Load()
    {
        string fullPath = GetFullPath();
        if (File.Exists(fullPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullPath, FileMode.Open);

            NinjasSaveData saveData = formatter.Deserialize(stream) as NinjasSaveData;
            stream.Close();

            return saveData;
        }

        return null;
    }

    private static string GetFullPath()
    {
        string partialPath = Application.persistentDataPath;
        string fullPath = partialPath + "/save.fun";
        return fullPath;
    }
}