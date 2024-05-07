using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

public class Match3Manager : MonoBehaviour
{
    public static Match3Manager Instance;

    FirebaseAuthManager firebaseAuthManager;
    LeaderboardManager leaderboardManager;

    [Header("UI")]
    public GameObject GameAgainButton;
    public GameObject PassedLevelButton;
    public GameObject LevelFrame;
    public GameObject SettingsFrame;
    public GameObject ElementParent;
    public GameObject LevelGrids;
    public Transform leaderboardPanel;
    public GameObject leaderboardPanelPrefab;
    public GameObject quitButton;
    public GameObject lbButton;
    [SerializeField] private Button _settingsButton;
    public Toggle LowToggle;
    public Toggle MediumToggle;
    public Toggle HighToggle;

    [Space(10)]

    public TextMeshProUGUI ScoreText;
    public int score;
    public TextMeshProUGUI SwapRightText;
    public int swapRight;
    public float delay;

    [Space(10)]

    [Header("Effects")]
    public GameObject boomEffect;
    public GameObject matchEffect;

    [Space(10)]
    public UnityEngine.UI.Slider slider;

    [HideInInspector] public BoxCollider2D Collider2D;
    [SerializeField] private GameAudioManager _audioManager;
    [SerializeField] private ElementBoard _elementBoard;


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

        UnityEngine.Cursor.visible = true;
        HighQuality();

    }

    public void Update()
    {
        SwapRightText.text = swapRight.ToString();
        ScoreText.text = score.ToString();

        if (swapRight <= 0)
        {
            swapRight = 0;

            if (!PassedLevelButton.activeInHierarchy)
                GameAgainButton.SetActive(true);

            ElementBoard.Instance.isProcessingMove = false;
            Collider2D.enabled = true;
        }

        if (LevelManager.Instance.PassedScore())
        {
            PassedLevelButton.SetActive(true);
            GameAgainButton.SetActive(false);

            Collider2D.enabled = true;
        }

        if (_elementBoard.isProcessingMove)
        {
            _settingsButton.interactable = false;
        }

    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("LoginScene");
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

    public void QuitButton()
    {
        Application.Quit();
    }



    public void SettingsBackButton()
    {
        if (LevelFrame.activeInHierarchy)
        {
            LevelFrame.SetActive(true);
        }
        else
        {
            ElementParent.SetActive(true);
            LevelGrids.SetActive(true);
        }

        if (!SettingsFrame.activeInHierarchy)
        {
            quitButton.SetActive(true);
            lbButton.SetActive(true);
        }
        else if (LevelGrids.activeInHierarchy)
        {
            quitButton.SetActive(false);
            lbButton.SetActive(false);
        }
    }

    #region Quality
    public void LowQuality()
    {
        QualitySettings.SetQualityLevel(0);
        HighToggle.isOn = false;
        MediumToggle.isOn = false;
    }

    public void MediumQuality()
    {
        QualitySettings.SetQualityLevel(1);
        LowToggle.isOn = false;
        HighToggle.isOn = false;
    }

    public void HighQuality()
    {
        QualitySettings.SetQualityLevel(2);
        LowToggle.isOn = false;
        MediumToggle.isOn = false;
    }
    #endregion

    #region Sound
    public void ButtonSound()
    {
        _audioManager.Button();
    }

    public void WinSound()
    {
        _audioManager.Win();
    }

    #endregion

    public IEnumerator SwapRightAmount()
    {
        yield return new WaitForSeconds(delay);
        _settingsButton.interactable = true;
        swapRight--;
    }

    public void ScoreSave()
    {
        firebaseAuthManager = FindAnyObjectByType<FirebaseAuthManager>();

        if (firebaseAuthManager != null && firebaseAuthManager.auth != null && firebaseAuthManager.auth.CurrentUser != null)
        {
            leaderboardManager = FindAnyObjectByType<LeaderboardManager>();

            leaderboardManager.ScoreData(firebaseAuthManager.auth.CurrentUser.DisplayName, score, LevelManager.Instance.currentLevel + 1);
            // StartCoroutine(LeaderboardManager.Instance.UpdateLeaderboard());
        }
        else
        {
            Debug.LogWarning("FirebaseAuthManager is not initialized or there is no authenticated user.");
        }
    }


    public void UpdateLB()
    {
        leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        leaderboardManager.UpdateLBAttributes(leaderboardPanel, leaderboardPanelPrefab);
    }
}
