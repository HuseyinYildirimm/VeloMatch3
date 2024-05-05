using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelID;
    public TMP_Text levelText;
    public Image lockedImage;
    public int rows;
    public int colums;

    private Button button;
    [SerializeField] private GameAudioManager _audioManager;

    public void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        levelText.text = levelID.ToString();
    }

    public void Update()
    {
        UnlockedLevel();
    }

    private void OnButtonClick()
    {
        ElementBoard.Instance.BoardCleaning();
        Match3Manager.Instance.GetComponent<Collider2D>().enabled = false;
        ElementBoard.Instance.InitializeBoard(rows, colums);
        LevelManager.Instance.currentLevel = levelID;
        LevelManager.Instance.SwapByLevel(levelID);

        _audioManager.Button();
    }

    void UnlockedLevel()
    {
        if (LevelManager.Instance.IsLevelLocked(levelID))
        {
            lockedImage.gameObject.SetActive(true);
            button.interactable = false;
        }
        else
        {
            lockedImage.gameObject.SetActive(false);
            button.interactable = true;
        }
    }
}
