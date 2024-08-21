using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float PlayerMoveSpeed = 5f;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Animation")]
    [SerializeField] public Animator animator;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    float horizantalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody PlayerRb;




    private void Awake()
    {
        PlayerRb = GetComponent<Rigidbody>();
        

    }

    private void Update()
    {
        GroundCheckRayCast();
        SpeedControl();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        PlayerMove();
        animator.SetFloat("vinput", horizantalInput, 0.1f, Time.fixedDeltaTime);
        animator.SetFloat("hzinput", verticalInput, 0.1f,Time.fixedDeltaTime);
    }



    private void PlayerMove()
    {
        //callculate movement direction
        horizantalInput = InputManager.Instance.HorizontalPos;
        verticalInput = InputManager.Instance.VerticalPos;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizantalInput;
        
        if (grounded)
        {
            
            PlayerRb.AddForce(moveDirection.normalized * PlayerMoveSpeed * 10f, ForceMode.Force);
            animator.SetBool("Walking", true);
        }
        else if (!grounded)
        {
            PlayerRb.AddForce(moveDirection.normalized * PlayerMoveSpeed * 10f * airMultiplier, ForceMode.Force);
            animator.SetBool("Walking", false);
        }

    }

    private void GroundCheckRayCast()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            PlayerRb.drag = groundDrag;
        }
        else
        {
            PlayerRb.drag = 0;
        }

    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);

        if (flatVel.magnitude > PlayerMoveSpeed)
        {
            Vector3 limetedVel = flatVel.normalized * PlayerMoveSpeed;
            PlayerRb.velocity = new Vector3(limetedVel.x, PlayerRb.velocity.y, limetedVel.z);
        }
    }
    private void Jump()
    {
        PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);
        PlayerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump Methodu");
    }
    private void ResetJump()
    {
        Debug.Log("JumpReset Methodu");
        readyToJump = true;
    }
    private void JumpCheck()
    {
        Debug.Log("JumpCheck Methodu");
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            Debug.Log("JumpCheck if Methodu");
        }
    }


}
