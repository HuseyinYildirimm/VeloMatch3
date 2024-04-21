using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text messageText;


    public GameObject Yellow;
    public GameObject Blue;
    public GameObject Green;
    public GameObject Purple;
    public GameObject Red;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;

        ShowMessage();
    }

    private void ShowMessage()
    {
        messageText.text = string.Format("Welcome, {0} In our game scene", References.userName);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

}
