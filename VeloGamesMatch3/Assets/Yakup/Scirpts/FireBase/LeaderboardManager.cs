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

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardPanel;
    public GameObject leaderboardPanelPrefab;

    DatabaseReference databaseReference;
    FirebaseAuth auth;
    private long previousUserScore = 0;
    private bool isListeningToDatabaseChanges = false;

    private void Start()
    {
        // Firebase bağımlılıklarının kontrol edilmesini bekle
        StartCoroutine(InitializeFirebase());
    }

    IEnumerator InitializeFirebase()
    {
        // Firebase bağımlılıklarının kontrol edilmesini bekle
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        // Bağımlılıklar başarıyla kontrol edildiyse Firebase veritabanı referansını al
        if (dependencyTask.Result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Kullanıcı oturum açıkken sadece onun skorlarını güncelle
            if (auth.CurrentUser != null)
            {
                StartListeningToDatabaseChanges();
                UpdateLeaderboard();
            }
        }
        else
        {
            Debug.LogError("Failed to check Firebase dependencies: " + dependencyTask.Result);
        }
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
            return;
        }

        if (this == null) return;

        var userScoreSnapshot = args.Snapshot.Child(auth.CurrentUser.UserId).Child("score");
        if (userScoreSnapshot.Exists)
        {
            long currentUserScore = Convert.ToInt64(userScoreSnapshot.Value);
            if (previousUserScore != currentUserScore)
            {
                StartCoroutine(UpdateLeaderboard());
                previousUserScore = currentUserScore;
            }
        }
    }

    public void AddScore(string playerName, long score)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be empty!");
            return;
        }

        ScoreEntry newEntry = new ScoreEntry(playerName, score, auth.CurrentUser.UserId);
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
        }
    }

    public IEnumerator UpdateLeaderboard()
    {
        var getScoresTask = databaseReference.Child("scores").OrderByChild("score").LimitToLast(10).GetValueAsync();

        yield return new WaitUntil(() => getScoresTask.IsCompleted);

        if (getScoresTask.Exception != null)
        {
            Debug.LogError(getScoresTask.Exception);
            yield break;
        }

        foreach (Transform child in leaderboardPanel)
        {
            Destroy(child.gameObject);
        }

        DataSnapshot snapshot = getScoresTask.Result;

        List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

        foreach (var childSnapshot in snapshot.Children)
        {
            string playerName = childSnapshot.Child("name").Value.ToString();
            long playerScore = (long)childSnapshot.Child("score").Value;
            string userId = childSnapshot.Key;
            scoreEntries.Add(new ScoreEntry(playerName, playerScore, userId));
        }

        scoreEntries = scoreEntries.OrderByDescending(entry => entry.score).ToList();

        foreach (var entry in scoreEntries)
        {
            GameObject panel = Instantiate(leaderboardPanelPrefab, leaderboardPanel);
            panel.transform.Find("NameText").GetComponent<Text>().text = entry.name;
            panel.transform.Find("ScoreText").GetComponent<Text>().text = entry.score.ToString();
        }
    }
}
