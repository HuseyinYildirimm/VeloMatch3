using UnityEngine;

public class TpsMovement : MonoBehaviour
{
    public Animator Anim;
    public float Speed = 2f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 direction = GetInputDirection();
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
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            Anim.SetBool("Running", false);
        }
    }
}
