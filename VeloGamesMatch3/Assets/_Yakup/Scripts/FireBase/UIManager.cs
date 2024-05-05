using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField] private GameObject loginPanel;

    [SerializeField] private GameObject registrationPanel;
    [SerializeField] private GameObject gamePanel;


    [Space]
    [SerializeField] private GameObject emailVerificationPanel;

    [SerializeField] private Text emailVerificationText;

    [Space]

    [Header("Profile Picture Update  Data ")]
    public GameObject profileUpdatePanel;
    public Image profileImage;
    public InputField urlInputField;

    public Text NameTxt;
    public Text ErrorTxT;
    private const float TextDuration = 5f;

    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;


    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ClearUI()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void OpenCloseProfileUpdatePanel()
    {
        profileUpdatePanel.SetActive(!profileUpdatePanel.activeSelf);
    }

    public void OpenLoginPanel()
    {
        ClearUI();
        loginPanel.SetActive(true);
    }
    public void OpenGamePanel()
    {
        ClearUI();
        gamePanel.SetActive(true);
    }
    public void OpenRegistrationPanel()
    {
        ClearUI();
        registrationPanel.SetActive(true);
    }

    public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage)
    {
        ClearUI();
        emailVerificationPanel.SetActive(true);

        if (isEmailSent)
        {
            emailVerificationText.text = $"Please verify your email address \n Verification email has been sent to {emailId}";
        }
        else
        {
            emailVerificationText.text = $"Couldn't sent email : {errorMessage}";
        }
    }
    public void LoadProfileImage(string url)
    {
        StartCoroutine(LoadProfileImageIE(url));
    }
    public IEnumerator LoadProfileImageIE(string url)
    {
        Debug.Log("Loading image from URL: " + url);


        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading image: " + www.error);
            ErrorTxT.text = "Error loading image: " + www.error;
            ActivateInfoText();
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
                profileImage.sprite = sprite;
                profileUpdatePanel.SetActive(false);
            }
            else
            {

                Debug.LogError("Downloaded texture is null");
                ErrorTxT.text = "Downloaded texture is null";
                ActivateInfoText();
            }
        }
    }

    public string GetUpdateURL()
    {
        return urlInputField.text;
    }

    public void ActivateInfoText()
    {
        StartCoroutine(ActivateInfoTextIE());
    }

    public IEnumerator ActivateInfoTextIE()
    {
        yield return new WaitForSeconds(TextDuration);

        ErrorTxT.text = null;
    }
}