using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour
{
    [SerializeField] private Text _InfoText;
    private const float TextDuration = 10f;


    private void Start()
    {
        _InfoText.gameObject.SetActive(true);
        StartCoroutine(ActivateInfoText());
    }


    private IEnumerator ActivateInfoText()
    {

        yield return new WaitForSeconds(TextDuration);
        _InfoText.gameObject.SetActive(false);
    }
}
