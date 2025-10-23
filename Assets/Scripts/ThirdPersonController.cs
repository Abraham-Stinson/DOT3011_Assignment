using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement and TPS Camera")]
    [SerializeField] private CharacterController chrachterController;
    [SerializeField, Range(0f, 100f)] private float speed = 10f;
    [SerializeField, Range(0f, 10f)] private float rotationSmoothTime = 0.1f;
    private float rotationVelocity;
    [SerializeField] Transform cam;

    [Header("Jump and Gravity")]
    [SerializeField, Range(0f, 100f)] private float gravity = -9.81f;
    [SerializeField, Range(0f, 50f)] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Interact")]
    [SerializeField, Range(0f, 100f)] private float camRange = 20f;


    void Update()
    {
        ApplyGravity();
        MovementHandle();
        JumpHandle();
        InteractHandle();
    }
    #region Movement and TPS Camera
    void MovementHandle()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angel = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angel, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            chrachterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
    #endregion
    #region Jump
    void JumpHandle()
    {
        //Debug.DrawRay(transform.position, Vector3.down * 1.1f, Color.red); DEBUG RAY
        if (Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground")))
        {
            gravity = jumpForce;
            chrachterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        }
    }
    #endregion
    #region Gravity
    void ApplyGravity()
    {
        if (chrachterController.isGrounded && gravity < 0)
        {
            gravity = -2f;
        }
        gravity += -9.81f * Time.deltaTime;
        chrachterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
    }
    #endregion

    #region Interact
    void InteractHandle() {
        Debug.DrawRay(cam.position, cam.forward * camRange, Color.red);
        if (Input.GetAxisRaw("Interact") > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, camRange))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
    #endregion

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand=GetComponentInChildren<IWeapon>();

            if (onHand != null)
            {
                Debug.Log("Weapon Fired!");
                onHand.Fire();
            }
        }
    }
    public void SwitchStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();

            if (onHand != null)
            {
                Debug.Log("Weapon Stance Switched");
                onHand.SwitchStance();
            }
        }
    }
}
