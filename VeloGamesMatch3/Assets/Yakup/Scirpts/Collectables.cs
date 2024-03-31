using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Collectables : MonoBehaviour, ICollectible
{

    [SerializeField] private GameObject _collectedParticle;
    private void Start()
    {
        _collectedParticle.SetActive(true);
    }

    public void Collect(GameObject players)
    {
        Debug.Log("ÇAĞIRDI");
        PlayerController player = players.GetComponent<PlayerController>();


        transform.SetParent(player.HandPos);
        transform.GetChild(0).localScale = new Vector3(50f, 50f, 50f);
        player.Animator.SetTrigger("isPicking");
        transform.DOMove(player.HandPos.position, 0.01f).SetEase(Ease.Linear).OnComplete(() =>
             {

                 Debug.Log("GİRDİ");
                 _collectedParticle.SetActive(false);

             });
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
}