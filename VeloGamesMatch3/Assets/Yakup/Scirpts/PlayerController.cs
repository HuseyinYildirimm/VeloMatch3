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
    GameManager gameManager;
    private void Start()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
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
                TpsMovement movement = GetComponent<TpsMovement>();
                movement.StopMoving();

                Vector3 lookDirection = (other.transform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0f, lookDirection.z), Vector3.up);
                transform.rotation = targetRotation;
                SetAnimator();
                isPicking = true;
                collectablesObj = other.GetComponent<Collectables>();
                ActiveUIRocks(other.gameObject);
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

    private void ActiveUIRocks(GameObject other)
    {

        if (other.gameObject.name == "Red")
        {
            gameManager.Red.SetActive(false);
        }

        if (other.gameObject.name == "Blue")
        {
            gameManager.Blue.SetActive(false);
        }


        if (other.gameObject.name == "Green")
        {
            gameManager.Green.SetActive(false);
        }


        if (other.gameObject.name == "Purple")
        {
            gameManager.Purple.SetActive(false);
        }


        if (other.gameObject.name == "Yellow")
        {
            gameManager.Yellow.SetActive(false);
        }

    }
}
