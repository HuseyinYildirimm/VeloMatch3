using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Collectables : MonoBehaviour, ICollectible
{

    [SerializeField] private GameObject _collectedParticle;
    public Transform GFX;
    private Tween _tween; // _tween değişkeni tanımlanır

    private void OnEnable()
    {
        StartTween();
    }

    private void OnDisable()
    {
        StopTween();
    }

    private void StartTween()
    {
        if (transform != null)
        {
            _tween = transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        if (_collectedParticle != null)
        {
            _collectedParticle.SetActive(true);
        }
    }

    private void StopTween()
    {
        if (_tween != null)
        {
            _tween.Kill(); // Tweeni durdur
        }
    }


    public void DeActiveParticle()
    {
        _collectedParticle.transform.localScale = _collectedParticle.transform.localScale / 2;
    }



    public void Collect(GameObject player)
    {

    }
}