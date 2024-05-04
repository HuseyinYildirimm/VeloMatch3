using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveLoginDoor : MonoBehaviour
{
    [SerializeField] private Animation _animationDoors;
    [SerializeField] private GameObject _collectedParticle;
    [SerializeField] private Animation _animationCircle;
    [SerializeField] private OpeningAudioManager _openingAudioManager;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] private PlayerController _playerController;
    public Transform[] UniqueRockPositions;
    public Transform[] Waypoints;

    public int RockPointCount = 0;
    public bool CanDrop = false;
    private const float MoveDuration = 2f;

    [Header("Elements")]
    public GameObject Yellow;
    public GameObject Blue;
    public GameObject Green;
    public GameObject Purple;
    public GameObject Red;

    private void Start()
    {
        CanDrop = false;
        _collectedParticle.SetActive(true);
    }

    public void OpenCaveDoorAnimations(Transform player)
    {
        StartCoroutine(OpenCaveDoorAnimationsIE(player));
    }

    IEnumerator OpenCaveDoorAnimationsIE(Transform player)
    {
        _animationCircle.Play("CaveLoginOpen");
        _openingAudioManager.Boom();
        yield return new WaitForSeconds(2);
        _animationCircle.Play("DownDoor");
        _openingAudioManager.Win();
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
                _playerController.DropUniqueRocks();
            }
        }
    }

    public void GoToCave(Transform player)
    {
        StartCoroutine(GoToCaveIE(player));
    }

    public IEnumerator GoToCaveIE(Transform player)
    {
        _virtualCamera.Follow = null;
        WaitForSeconds waitForSeconds = new WaitForSeconds(MoveDuration);
        _playerController.Anim.SetBool("Running", true);

        foreach (Transform wayPoint in Waypoints)
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
