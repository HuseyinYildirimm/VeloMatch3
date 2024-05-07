using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardPanel;
    public GameObject leaderboardPanelPrefab;

    public DatabaseReference databaseReference;
    FirebaseAuth auth;
    private long previousUserScore = 0;
    private bool isListeningToDatabaseChanges = false;
    private int rankCounter = 1;


    public static LeaderboardManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(InitializeFirebase());
    }

    IEnumerator InitializeFirebase()
    {

        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);


        if (dependencyTask.Result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;


            if (auth.CurrentUser != null)
            {
                StartListeningToDatabaseChanges();
                StartCoroutine(UpdateLeaderboard(leaderboardPanel, leaderboardPanelPrefab));
            }
        }
        else
        {
            Debug.LogError("Failed to check Firebase dependencies: " + dependencyTask.Result);
            UIManager.Instance.ErrorTxT.text = "Failed to check Firebase dependencies: " + dependencyTask.Result;

        }
    }

    public void UpdateLB()
    {
        StartCoroutine(UpdateLeaderboard(leaderboardPanel, leaderboardPanelPrefab));
    }
    public void UpdateLBAttributes(Transform leaderboardPanel, GameObject leaderboardPanelPrefab)
    {
        StartCoroutine(UpdateLeaderboard(leaderboardPanel, leaderboardPanelPrefab));
    }

    private void StartListeningToDatabaseChanges()
    {
        if (!isListeningToDatabaseChanges)
        {
            FirebaseDatabase.DefaultInstance.GetReference("scores").ValueChanged += HandleValueChanged;
            isListeningToDatabaseChanges = true;
        }
    }

    private void StopListeningToDatabaseChanges()
    {
        if (isListeningToDatabaseChanges)
        {
            FirebaseDatabase.DefaultInstance.GetReference("scores").ValueChanged -= HandleValueChanged;
            isListeningToDatabaseChanges = false;
        }
    }

    private void OnDestroy()
    {
        StopListeningToDatabaseChanges();
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            UIManager.Instance.ErrorTxT.text = args.DatabaseError.Message;
            return;
        }

        if (this == null) return;

        var userScoreSnapshot = args.Snapshot.Child(auth.CurrentUser.UserId).Child("score");
        if (userScoreSnapshot.Exists)
        {
            long currentUserScore = Convert.ToInt64(userScoreSnapshot.Value);
            if (previousUserScore != currentUserScore)
            {
                StartCoroutine(UpdateLeaderboard(leaderboardPanel, leaderboardPanelPrefab));
                previousUserScore = currentUserScore;
            }
        }
    }

    public void ScoreData(string playerName, long score, int level)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be empty!");
            UIManager.Instance.ErrorTxT.text = "Player name cannot be empty!";
            return;
        }

        ScoreEntry newEntry = new ScoreEntry(playerName, score, auth.CurrentUser.UserId, level);

        UpdateUserScore(newEntry);
    }

    private void UpdateUserScore(ScoreEntry newEntry)
    {
        var updateTask = databaseReference.Child("scores").Child(auth.CurrentUser.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(newEntry));
        StartCoroutine(WaitForTaskCompletion(updateTask));
    }

    IEnumerator WaitForTaskCompletion(Task task)
    {
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
            UIManager.Instance.ErrorTxT.text = task.Exception.ToString();
        }
    }

    public IEnumerator UpdateLeaderboard(Transform leaderboardPanel, GameObject leaderboardPanelPrefab)
    {
        var getScoresTask = databaseReference.Child("scores").OrderByChild("score").LimitToLast(10).GetValueAsync();


        yield return new WaitUntil(() => getScoresTask.IsCompleted);

        if (getScoresTask.Exception != null)
        {
            Debug.LogError(getScoresTask.Exception);
            yield break;
        }
        if (leaderboardPanel != null)
        {
            foreach (Transform child in leaderboardPanel)
            {
                Destroy(child.gameObject);
            }
        }
        DataSnapshot snapshot = getScoresTask.Result;
        List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

        foreach (var childSnapshot in snapshot.Children)
        {
            string playerName = childSnapshot.Child("name").Value.ToString();
            long playerScore = (long)childSnapshot.Child("score").Value;
            string userId = childSnapshot.Key;
            int playerLevel = Convert.ToInt32(childSnapshot.Child("level").Value);
            scoreEntries.Add(new ScoreEntry(playerName, playerScore, userId, playerLevel));
        }

        scoreEntries = scoreEntries.OrderByDescending(entry => entry.score).ToList();

        rankCounter = 1;
        foreach (var entry in scoreEntries)
        {

            GameObject panel = Instantiate(leaderboardPanelPrefab, leaderboardPanel);
            panel.GetComponent<ScorePF>().RankTxt.text = rankCounter.ToString() + ".";
            panel.GetComponent<ScorePF>().NameTxt.text = entry.name;
            panel.GetComponent<ScorePF>().ScoreTxt.text = entry.score.ToString();
            panel.GetComponent<ScorePF>().LevelTxt.text = "Lvl: " + entry.level.ToString();
            rankCounter++;
        }
    }

    public void GetPassedValue(string userId, Action<int> callback)
    {
        // Query Firebase database to get user's 'passed' value
        databaseReference.Child("scores").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                // Task completed successfully
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.HasChild("passed"))
                {
                    int passed = Convert.ToInt32(snapshot.Child("passed").Value);
                    callback(passed);
                }
                else
                {
                    // 'passed' value not found
                    Debug.LogWarning("User's 'passed' value not found in the database.");
                    callback(-1); // You may handle this case differently based on your application's logic
                }
            }
            else if (task.IsFaulted)
            {
                // Task failed
                Debug.LogWarning("Task failed: " + task.Exception);
                callback(-1); // You may handle this case differently based on your application's logic
            }
        });
    }
}

