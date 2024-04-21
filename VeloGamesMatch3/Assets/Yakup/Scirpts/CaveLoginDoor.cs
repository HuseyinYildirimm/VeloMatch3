using System.Collections;

using Cinemachine;
using DG.Tweening;
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
    private const float MoveDuration = 2f;



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
        WaitForSeconds waitForSeconds = new WaitForSeconds(MoveDuration);
        playerController.Anim.SetBool("Running", true);

        foreach (Transform wayPoint in wayPoints)
        {
            Vector3 lookDirection = (wayPoint.position - player.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            player.rotation = targetRotation;

            player.DOMove(wayPoint.position, MoveDuration).SetEase(Ease.Linear);
            yield return waitForSeconds;
        }

        SceneManager.LoadScene("GameScene");
    }

}