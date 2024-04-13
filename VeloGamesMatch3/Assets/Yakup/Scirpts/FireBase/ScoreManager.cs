 using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;
    public FirebaseAuthManager firebaseAuthManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (leaderboardManager != null)
            {
                AddScorer();
                leaderboardManager.UpdateLeaderboard();
            }
        }
    }
    private void Start()
    {
        // firebaseAuthManager nesnesinin null olup olmadığını kontrol et
        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            // firebaseAuthManager.auth.CurrentUser değerini kullanarak skorları güncelle
            leaderboardManager.UpdateLeaderboard();
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }

    public void AddScorer()
    {
        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            // firebaseAuthManager.auth.CurrentUser.DisplayName değerini kullanarak bir oyuncu skoru ekleyin
            leaderboardManager.AddScore(firebaseAuthManager.auth.CurrentUser.DisplayName, Random.Range(0, 100));
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }
}
