using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelID;
    public bool isLocked;
    public int levelScore;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private List<Level> levels;
    private void Awake()
    {
        Instance = this;
    }

    public void UnlockLevel(int levelID)
    {
        Level tempLevel = levels.Find(level => level.levelID == levelID);

        if (tempLevel != null)
        {
            tempLevel.isLocked = false;

        }
    }

    public bool IsLevelLocked(int levelID)
    {
        Level tempLevel = levels.Find(level => level.levelID == levelID);

        if (tempLevel != null)
        {
            return tempLevel.isLocked = false;

        }return false;
    }


}
