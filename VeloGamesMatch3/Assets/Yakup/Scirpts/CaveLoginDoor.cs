using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CaveLoginDoor : MonoBehaviour
{

    [SerializeField] private GameObject _collectedParticle;
    [SerializeField] private Animation _animatonCircle;
    [SerializeField] private Animation _animationDoors;


    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public Transform[] UniqeRockPostions;
    public Transform[] wayPoints;
    PlayerController playerController;
    public int rockPointCount = 0;
    public bool CanDrop = false;



    private void Start()
    {
        CanDrop = false;
        _collectedParticle.SetActive(true);
        playerController = FindAnyObjectByType<PlayerController>();
    }

    public void OpenCaveDoorAnimations(Transform player)
    {
        StartCoroutine(OpenCaveDoorAnimationsIE(player));
    }
    IEnumerator OpenCaveDoorAnimationsIE(Transform player)
    {
        _animatonCircle.Play("CaveLoginOpen");
        yield return new WaitForSeconds(2);
        _animatonCircle.Play("DownDoor");
        yield return new WaitForSeconds(2);
        _animationDoors.Play();
        yield return new WaitForSeconds(2);
        GoToCave(player);


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            if (CanDrop)
            {
                playerController.DropUnqieRocks();
            }
        }
    }
    public void GoToCave(Transform player)
    {
        StartCoroutine(GoToCaveIE(player));
    }
    public IEnumerator GoToCaveIE(Transform player)
    {
        virtualCamera.Follow = null;
        WaitForSeconds waitForSeconds = new WaitForSeconds(2f);
        playerController.Anim.SetBool("Running", true);
        int cnt = 0;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            cnt++;
            Vector3 lookDirection = (wayPoints[i].transform.position - player.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            player.transform.rotation = targetRotation;


            player.DOMove(wayPoints[i].transform.position, 2f).SetEase(Ease.Linear);
            if (cnt >= 3)
            {
                SceneManager.LoadScene("GameScene");
            }
            yield return waitForSeconds;

        }


    }


}