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
    [SerializeField] public CharacterController characterController;
    [SerializeField, Range(0f, 100f)] private float speed = 10f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothTime = 0.5f;
    private float rotationVelocity;
    [SerializeField] Transform cam;

    [Header("Jump and Gravity")]
    [SerializeField, Range(-100f, 100f)] private float gravity = -9.81f;
    [SerializeField, Range(0f, 50f)] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(1f, 2f)] private float groundRange;
    [Header("Interact")]
    [SerializeField, Range(0f, 100f)] private float camRange = 20f;
    [SerializeField, Range(0f, 100f)] private float camStartOffset = 1f;
    /*[SerializeField, Range(0f, 100f)] private float camStartOrginX = 10f;
    [SerializeField, Range(0f, 100f)] private float camStartOrginY = 10f;
    [SerializeField, Range(0f, 100f)] private float camStartOrginZ = 10f;*/
    public PlayerInputActions playerInputActions;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    public bool isAttacking;
    void Awake()
    {
        //playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        //playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += JumpHandle;
        playerInputActions.Player.Interact.performed += InteractHandle;
        playerInputActions.Player.MenuTrigger.performed += MenuToggle;
        playerInputActions.Player.MainAttack.performed += MainAttack;
        playerInputActions.Player.SecondaryAttack.performed += SecondaryAttack;
        //playerInputActions.Player.Movement.performed += MovementHandle_performed;

        instance = this;

    }
    void Update()
    {
        ApplyGravity();
        RotatePlayerToCamera();
        //MovementHandle();
        //JumpHandle();
        //InteractHandle();
    }
    void FixedUpdate()
    {
        MovementHandle();
        CameraRay();

    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
    }
    #region Movement and TPS Camera
    private void MovementHandle()
    {
        if (isAttacking) return;

        Vector2 input = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(input.x, 0f, input.y);

        if (moveDir.magnitude >= 0.1f)
        {
            if (animator != null)
                animator.SetBool("isMoving", true);

            // Move relative to camera
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0f;
            camRight.y = 0f;

            Vector3 finalMove =
                camForward.normalized * moveDir.z +
                camRight.normalized * moveDir.x;

            characterController.Move(finalMove * speed * Time.deltaTime);
        }
        else
        {
            if (animator != null)
                animator.SetBool("isMoving", false);
        }
    }


    private void RotatePlayerToCamera()
    {
        Vector3 camForward = cam.forward;
        camForward.y = 0f; // ignore vertical tilt

        if (camForward.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime / rotationSmoothTime
        );
    }
    #endregion
    #region Jump
    public void JumpHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isAttacking) return;

        if (isPlayerOnGround())
        {
            gravity = jumpForce;
            characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
            if (animator != null)
                animator.SetBool("isJumping", true);
        }

        //Debug.DrawRay(transform.position, Vector3.down * 1.1f, Color.red); DEBUG RAY
        /*if (Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground")))
        {
            gravity = jumpForce;
            characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        }*/
    }

    bool isPlayerOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundRange, LayerMask.GetMask("Ground"));
    }

    /*private JumpPerformed JumpPerformed(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }*/
    #endregion
    #region Gravity
    void ApplyGravity()
    {

        if (isPlayerOnGround() && gravity < 0)
        {
            gravity = -2f;
            if (isPlayerOnGround())
            {
                if (animator != null)
                {
                    animator.SetBool("isFalling", false);
                    animator.SetBool("isJumping", false);
                }

            }

        }
        else
        {

            if (gravity < 0 && !isPlayerOnGround())
            {
                if (animator != null)
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isFalling", true);
                }

            }
        }
        gravity += -9.81f * Time.deltaTime;

        characterController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
    }
    #endregion

    #region Interact
    void InteractHandle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;


        RaycastHit hit;
        if (Physics.Raycast(GetRayStartOrgin() /*+ new Vector3(camStartOrginX,camStartOrginY,camStartOrginZ)*/, cam.forward, out hit, camRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
    private Vector3 GetRayStartOrgin()
    {
        return cam.position + (cam.forward * camStartOffset);
    }
    private void CameraRay()
    {
        Debug.DrawRay(GetRayStartOrgin() /*+ new Vector3(camStartOrginX,camStartOrginY,camStartOrginZ)*/, cam.forward * camRange, Color.red);
    }
    #endregion
    public void MenuToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Menu Toggle Triggered");
        UIManager.instance.PauseMenuToggle();
    }

    public void MainAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();
            if (onHand != null)
            {
                Debug.Log("Weapon Main Attack Fired!");
                onHand.MainAttack();
            }
            else
            {
                Debug.Log("NO WEAPON ON HAND!");
            }
        }
    }
    public void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();

            if (onHand != null)
            {
                Debug.Log("Weapon Secondary Attack Fired!");
                onHand.SecondaryAttack();
            }
            else
            {
                Debug.Log("NO WEAPON ON HAND!");
            }
        }
    }
    public void UltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IWeapon onHand = GetComponentInChildren<IWeapon>();

            if (onHand != null)
            {
                Debug.Log("Weapon ULTIMATE ATTACK Fired!");
                onHand.UltimateAttack();
            }
            else
            {
                Debug.Log("NO WEAPON ON HAND!");
            }
            
        }
    }

    public void GetAnimatorCompononet()
    {
        animator = GetComponentInChildren<Animator>();
    }


}
