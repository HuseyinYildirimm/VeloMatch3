using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingFrame : MonoBehaviour
{
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image loaddingBarFill;

    public void LoadScene()
    {
        ActivePanel();
        StartCoroutine(LoadSceneAsync());
    }

    public void ActivePanel()
    {
        LoadingScreen.gameObject.SetActive(true);
        loaddingBarFill.gameObject.SetActive(true);
    }

    IEnumerator LoadSceneAsync()
    {
        while (loaddingBarFill.fillAmount < 1f)
        {
            loaddingBarFill.fillAmount += Time.deltaTime * 0.5f; 
            yield return null;
        }
    }
}