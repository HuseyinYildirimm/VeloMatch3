using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform HandPos;
    public Animator Anim;
    public CaveLoginDoor CaveLoginDoor;
    public int UniqueRockCount = 5;
    private const float ActivationDelay = 1.4f;

    public bool IsPicking = false;

    private Collectables _collectablesObj;
    [SerializeField] private CaveLoginDoor _caveLoginDoor;
   // OpeningAudioManager audioManager;

    private void Awake()
    {
        //audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<OpeningAudioManager>();
    }
   
    public void SetAnimator()
    {
        Anim.SetTrigger("IsPicking");
    }

    public void Collect(GameObject obj)
    {
        if (IsPicking && !CaveLoginDoor.CanDrop)
        {
            _collectablesObj.transform.SetParent(HandPos);
            _collectablesObj.GFX.localScale = new Vector3(50f, 50f, 50f);
            _collectablesObj.transform.DOMove(HandPos.position, 0.01f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Debug.Log("Collect");
                    //audioManager.PlaySFX(audioManager.gem1);
                    CaveLoginDoor.CanDrop = true;
                    IsPicking = false;
                });
        }
    }

    public void DropUniqueRocks()
    {
        if (CaveLoginDoor.CanDrop && !IsPicking)
        {
            int i = CaveLoginDoor.RockPointCount;
            if (i > 5)
                return;
            TpsMovement movement = GetComponent<TpsMovement>();
            movement.StopMoving();

            _collectablesObj.transform.DOMove(CaveLoginDoor.UniqueRockPositions[i].position, 1.5f)
                .SetEase(Ease.OutFlash)
                .OnComplete(() =>
                {
                    Debug.Log("Drop");

                    _collectablesObj.transform.SetParent(CaveLoginDoor.transform.GetChild(0));
                    _collectablesObj.GFX.localScale = new Vector3(70f, 70f, 70f);

                    CaveLoginDoor.RockPointCount++;
                    CaveLoginDoor.CanDrop = false;

                    _collectablesObj.DeActiveParticle();
                    _collectablesObj.GetComponent<CapsuleCollider>().enabled = false;

                    movement.IsMoving = true;
                    Anim.SetBool("Running", true);

                    if (CaveLoginDoor.RockPointCount > 4)
                    {
                        movement.StopMoving();
                        CaveLoginDoor.OpenCaveDoorAnimations(transform);
                    }
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectables"))
        {
            if (!CaveLoginDoor.CanDrop)
            {
                TpsMovement movement = GetComponent<TpsMovement>();
                movement.StopMoving();

                Vector3 lookDirection = (other.transform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0f, lookDirection.z), Vector3.up);
                transform.rotation = targetRotation;
                SetAnimator();
                IsPicking = true;
                _collectablesObj = other.GetComponent<Collectables>();
                StartCoroutine(ActivateRockIE(other.gameObject));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectables"))
        {
            IsPicking = false;
        }
    }

    private IEnumerator ActivateRockIE(GameObject rock)
    {
        string name = rock.gameObject.name;
        yield return new WaitForSeconds(ActivationDelay);
        switch (name)
        {
            case "RED":
                _caveLoginDoor.Red.SetActive(true);
                break;
            case "BLUE":
                _caveLoginDoor.Blue.SetActive(true);
                break;
            case "GREEN":
                _caveLoginDoor.Green.SetActive(true);
                break;
            case "PURPLE":
                _caveLoginDoor.Purple.SetActive(true);
                break;
            case "YELLOW":
                _caveLoginDoor.Yellow.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown rock color!");
                break;
        }
    }
}
