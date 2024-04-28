using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System;
using System.Collections.Generic;

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
    DatabaseReference databaseReference;
    [SerializeField] private List<Level> levels;

    public int currentLevel = 1;

    private void Awake()
    {
        Instance = this;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        StartCoroutine(InitializeLevel());
    }

    IEnumerator InitializeLevel()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var getUserLevelTask = databaseReference.Child("scores").Child(userId).Child("level").GetValueAsync();
        yield return new WaitUntil(() => getUserLevelTask.IsCompleted);

        if (getUserLevelTask.Exception != null)
        {
            Debug.LogError(getUserLevelTask.Exception);
            yield break;
        }

        DataSnapshot userLevelSnapshot = getUserLevelTask.Result;
        if (userLevelSnapshot.Exists)
        {
            int playerLevel = Convert.ToInt32(userLevelSnapshot.Value);
            currentLevel = playerLevel;
        }

        if (levels.Count > currentLevel)
        {
            for (int i = 0; i < currentLevel; i++)
            {
                levels[i].isLocked = false;
            }
        }
        else
        {
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].isLocked = false;
            }
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

    public void UnlockLevel(int _levelID)
    {
        Level tempLevel = levels.Find(level => level.levelID == _levelID);

        if (tempLevel != null)
        {
            tempLevel.isLocked = false;
        }
    }

    public bool IsLevelLocked(int _levelID)
    {
        Level tempLevel = levels.Find(level => level.levelID == _levelID);
        if (tempLevel != null)
        {
            return tempLevel.isLocked;
        }
        return false;
    }

    public void SwapByLevel(int _levelID)
    {
        Level tempLevel = levels.Find(level => level.levelID == _levelID);

        if (tempLevel != null)
            GameManager1.Instance.swapRight = tempLevel.swapRight;
    }

    public bool PassedScore()
    {
        Level tempLevel = levels.Find(level => level.levelID == currentLevel);
        if (tempLevel != null)
        {
            return GameManager1.Instance.score >= tempLevel.levelScore;
        }
        return false;
    }
}
