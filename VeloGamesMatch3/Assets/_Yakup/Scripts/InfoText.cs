using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour
{
    [SerializeField] private Text _InfoText;
    [SerializeField] private float TextDuration = 10f;

    private bool infoTextCoroutineRunning = false;

    private void Start()
    {
        _InfoText.gameObject.SetActive(true);
        StartCoroutine(ActivateInfoText());
    }


    public IEnumerator ActivateInfoText()
    {
        if (infoTextCoroutineRunning)
            yield break;

        infoTextCoroutineRunning = true;

        yield return new WaitForSeconds(TextDuration);

        _InfoText.text = null;
        infoTextCoroutineRunning = false;
    }
}
