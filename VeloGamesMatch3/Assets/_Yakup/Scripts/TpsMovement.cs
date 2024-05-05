using UnityEngine;

public class TpsMovement : MonoBehaviour
{
    public Animator Anim;
    public float Speed = 2f;
    public float RotationSpeed = 5f;
    public bool IsMoving = true;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        IsMoving = true;
    }

    void Update()
    {
        Vector3 direction = GetInputDirection();

        if (!IsMoving)
            return;
        MoveCharacter(direction);



    }

    private Vector3 GetInputDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        return new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void MoveCharacter(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            Anim.SetBool("Running", true);
            rb.velocity = direction * Speed;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * RotationSpeed));
        }
        else
        {
            Anim.SetBool("Running", false);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 5 * Time.deltaTime);
        }
    }

    public void CanMoveTrue()
    {
        IsMoving = true;

    }
    public void CanMoveFalse()
    {
        IsMoving = false;

    }
    public void StopMoving()
    {
        IsMoving = false;
        Anim.SetBool("Running", false);
        rb.velocity = Vector3.zero;
    }
}
