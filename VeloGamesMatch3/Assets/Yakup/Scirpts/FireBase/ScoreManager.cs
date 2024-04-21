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


    public void AddScorer()
    {
        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            // firebaseAuthManager.auth.CurrentUser.DisplayName değerini kullanarak bir oyuncu skoru ekleyin
            leaderboardManager.AddScore(firebaseAuthManager.auth.CurrentUser.DisplayName, Random.Range(0, 100), Random.Range(0, 5));
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }
}
