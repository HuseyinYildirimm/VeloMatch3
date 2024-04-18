using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Collectables : MonoBehaviour, ICollectible
{

    [SerializeField] private GameObject _collectedParticle;
    public Transform GFX;



    private void Start()
    {


        transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        _collectedParticle.SetActive(true);

    }

    public void DeActiveParticle()
    {
        _collectedParticle.transform.localScale = _collectedParticle.transform.localScale / 2;
    }



    public void Collect(GameObject player)
    {

    }
}