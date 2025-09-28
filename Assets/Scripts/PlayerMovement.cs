using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Referances")]
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField, Range(-90, 90)] private float minCamLimit = -60f;
    [SerializeField, Range(-90, 90)] private float maxCamLimit = 60f;

    [Header("TPS References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private float rotationSpeed = 2f;


    void Start()
    {
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        //HandleTPSCamera();

    }
    void FixedUpdate()
    {
        HandleTPSCamera();
    }

    #region Movement
    void HandleMovement()//MOVEMENT
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed, 0, 0));
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(new Vector3(0, 0, Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed));
        }
    }
    #endregion
    #region Jump
    void HandleJump()//JUMP
    {
        Debug.DrawRay(playerObj.position, Vector3.down * 1.1f, Color.red);//raycast jumpable ground
        if (Physics.Raycast(playerObj.position, Vector3.down, 1.1f, groundLayer) && Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }
    #endregion
    #region TPS Camera
    void HandleTPSCamera()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * vertical + orientation.right * horizontal;

        if(inputDir!= Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }
    #endregion
}
