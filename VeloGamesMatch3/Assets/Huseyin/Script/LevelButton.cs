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
        GameManager1.Instance.collider2D.enabled = false;
        ElementBoard.Instance.InitializeBoard(rows, colums);
        LevelManager1.Instance.currentLevel = levelID;
        LevelManager1.Instance.SwapByLevel(levelID);
    }

    void UnlockedLevel()
    {
        if (LevelManager1.Instance.IsLevelLocked(levelID))
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
