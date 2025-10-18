using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ThirdPersonController : MonoBehaviour
{
    public static ThirdPersonController instance { get; private set; }
    //private PlayerInput playerInput;
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
    public PlayerInputActions playerInputActions;
    void Awake()
    {
        //playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        //playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += JumpHandle;
        playerInputActions.Player.Interact.performed += InteractHandle;
        //playerInputActions.Player.Movement.performed += MovementHandle_performed;

        instance = this;

    }
    void Update()
    {
        ApplyGravity();
        //MovementHandle();
        //JumpHandle();
        //InteractHandle();
    }
    void FixedUpdate()
    {
        MovementHandle();
        Debug.DrawRay(cam.position, cam.forward * camRange, Color.red);
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
    }
    #region Movement and TPS Camera
    void MovementHandle()
    {
        /*float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");*/

        Vector2 movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        float horizontal = movementInput.x;
        float vertical = movementInput.y;
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
    public void JumpHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground")))
        {
            gravity = jumpForce;
            chrachterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        }

        //Debug.DrawRay(transform.position, Vector3.down * 1.1f, Color.red); DEBUG RAY
        /*if (Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground")))
        {
            gravity = jumpForce;
            chrachterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        }*/
    }

    /*private JumpPerformed JumpPerformed(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }*/
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
    void InteractHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;


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
    #endregion
}
