using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;
    FirebaseAuthManager firebaseAuthManager;
    LeaderboardManager leaderboardManager;
    public GameObject GameAgainButton;
    public GameObject PassedLevelButton;
    public GameObject LevelFrame;

    public TextMeshProUGUI ScoreText;
    public int score;

    public TextMeshProUGUI SwapRightText;
    public int swapRight;

    public float delay;
    [HideInInspector] public BoxCollider2D Collider2D;


    public void Awake()
    {
        Instance = this;
        Collider2D = GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        Collider2D.enabled = false;
        GameAgainButton.SetActive(false);
        PassedLevelButton.SetActive(false);
    }

    public void Update()
    {
        SwapRightText.text = swapRight.ToString();
        ScoreText.text = score.ToString();

        if (swapRight <= 0)
        {
            swapRight = 0;
            GameAgainButton.SetActive(true);
            ElementBoard.Instance.isProcessingMove = false;
        }

        if (LevelManager1.Instance.PassedScore())
        {
            PassedLevelButton.SetActive(true);
            Collider2D.enabled = true;
        }
    }

    public void PassedLevel()
    {
        LevelFrame.SetActive(true);
        ScoreSave();
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
        firebaseAuthManager = FindAnyObjectByType<FirebaseAuthManager>();

        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
            leaderboardManager.ScoreData(firebaseAuthManager.auth.CurrentUser.DisplayName, score, LevelManager1.Instance.currentLevel +1 );
           // StartCoroutine(LeaderboardManager.Instance.UpdateLeaderboard());
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }

    
}
