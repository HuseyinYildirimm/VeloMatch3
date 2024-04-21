using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform HandPos;
    public Animator Anim;
    public CaveLoginDoor caveLoginDoor;
    public int uniqRockCount = 5;

    public bool isPicking = false;

    Collectables collectablesObj;

    OpeningAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<OpeningAudioManager>();

    }
    public void SetAnimator()
    {
        Anim.SetTrigger("isPicking");
    }
    public void Collect(GameObject obj)
    {
        if (isPicking && !caveLoginDoor.CanDrop)
        {
            collectablesObj.transform.SetParent(HandPos);
            collectablesObj.GFX.localScale = new Vector3(50f, 50f, 50f);
            collectablesObj.transform.DOMove(HandPos.position, 0.01f).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     Debug.Log("Collect");
                     audioManager.PlaySFX(audioManager.gem1);
                     caveLoginDoor.CanDrop = true;
                     isPicking = false;
                 });
        }


    }

    public void DropUnqieRocks()
    {
        if (caveLoginDoor.CanDrop && !isPicking)
        {
            int i = caveLoginDoor.rockPointCount;
            if (i > 5)
                return;
            TpsMovement movement = GetComponent<TpsMovement>();
            movement.StopMoving();

            collectablesObj.transform.DOMove(caveLoginDoor.UniqeRockPostions[i].position, 1.5f).SetEase(Ease.OutFlash).OnComplete(() =>
                     {
                         Debug.Log("Drop");
                         collectablesObj.transform.SetParent(caveLoginDoor.transform.GetChild(0));
                         collectablesObj.GFX.localScale = new Vector3(70f, 70f, 70f);
                         caveLoginDoor.rockPointCount++;
                         caveLoginDoor.CanDrop = false;
                         collectablesObj.DeActiveParticle();
                         collectablesObj.GetComponent<CapsuleCollider>().enabled = false;
                         movement.isMoving = true;
                         Anim.SetBool("Running", true);
                         if (caveLoginDoor.rockPointCount > 4)
                         {                          
                             movement.StopMoving();
                             caveLoginDoor.OpenCaveDoorAnimations(transform);

                         }

                     }); ;

        }


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Collectables"))
        {

            if (!caveLoginDoor.CanDrop)
            {
                SetAnimator();
                isPicking = true;
                collectablesObj = other.GetComponent<Collectables>();

            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectables"))
        {
            isPicking = false;
        }
    }
}
