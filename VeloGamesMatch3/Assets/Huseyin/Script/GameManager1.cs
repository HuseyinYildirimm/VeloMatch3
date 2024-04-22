using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    public GameObject GameAgainButton;
    public GameObject PassedLevelButton;
    public GameObject LevelFrame;

    public TextMeshProUGUI ScoreText;
    public int score;

    public TextMeshProUGUI SwapRightText;
    public int swapRight;

    public float delay;
    [HideInInspector] public BoxCollider2D collider2D;


    public void Awake()
    {
        Instance = this;
        collider2D = GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        collider2D.enabled = false;
        GameAgainButton.SetActive(false);
        PassedLevelButton.SetActive(false);
    }

    public void Update()
    {
        SwapRightText.text = swapRight.ToString();
        ScoreText.text = score.ToString();

        if(swapRight <= 0)
        {
            swapRight = 0;
            GameAgainButton.SetActive(true);
            ElementBoard.Instance.isProcessingMove = false;
        }

        if (LevelManager1.Instance.PassedScore())
        {
            PassedLevelButton.SetActive(true);
            collider2D.enabled = true;
        }
    }

    public void PassedLevel()
    {
        LevelFrame.SetActive(true);
    }

    public void GameAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public IEnumerator SwapRightAmount()
    {
        yield return new WaitForSeconds(delay);
        swapRight--;
    }

    public void ScoreSave()
    {
        FirebaseAuthManager firebaseAuthManager = FirebaseAuthManager.Instance;
        LeaderboardManager leaderboardManager = LeaderboardManager.Instance;
        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            // firebaseAuthManager.auth.CurrentUser.DisplayName deðerini kullanarak bir oyuncu skoru ekleyin
            leaderboardManager.ScoreData(firebaseAuthManager.auth.CurrentUser.DisplayName, score , LevelManager1.Instance.currentLevel);
            StartCoroutine(leaderboardManager.UpdateLeaderboard());
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }
}
