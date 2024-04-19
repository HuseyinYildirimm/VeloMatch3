using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    public GameObject GameAgainButton;

    public TextMeshProUGUI ScoreText;
    public int score;

    public TextMeshProUGUI SwapRightText;
    public int swapRight;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        GameAgainButton.SetActive(false);
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
    }


    public void GameAgain()
    {
        Time.timeScale = 1f;
        swapRight = 2;
        SceneManager.LoadScene("Huseyin");
    }

    public IEnumerator SwapRightAmount(float delay)
    {
        yield return new WaitForSeconds(delay);
        swapRight--;
    }

}
