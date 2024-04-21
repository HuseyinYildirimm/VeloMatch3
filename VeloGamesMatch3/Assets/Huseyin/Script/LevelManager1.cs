using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelID;
    public bool isLocked;
    public int levelScore;
    public int swapRight;
}

public class LevelManager1 : MonoBehaviour
{
    public static LevelManager1 Instance;
    public int currentLevel;

    [SerializeField] private List<Level> levels;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < currentLevel; i++)
        {
            levels[i].isLocked = false;
        }
    }

    public void Update()
    {
        Level temp = levels.Find(level => level.levelID == currentLevel);

        if (temp != null && temp.levelScore <= GameManager1.Instance.score)
        {
            UnlockLevel(currentLevel + 1);
        }

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
            return tempLevel.isLocked;

        }return false;
    }


}
