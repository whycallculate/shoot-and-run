using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using Cinemachine;
public enum MovementState
{
    WALK,
    SPRINTING,
    AIR,
    CROUCHING
}


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float PlayerMoveSpeed = 5f;
    public float playerWalkSpeed;
    public float playerSprintSpeed;
    public MovementState state;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Animation")]
    [SerializeField] public Animator animator;
    PhotonView pw;
    PhotonAnimatorView animpw;
    CinemachineImpulseSource movementShake;


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
        pw = GetComponent<PhotonView>();
        movementShake = GetComponent<CinemachineImpulseSource>();
        // Eğer bu oyuncu local oyuncu değilse, script'i devre dışı bırak
        if (!pw.IsMine)
        {
            this.enabled = false;
        }

    }
    private void Start()
    {
        if (pw.IsMine)
        {
            // Hareket kodlarınız burada
            startYScale = transform.localScale.y;
        }

        
    }

    private void Update()
    {
        if (pw.IsMine)
        {
            InputManager.MoveInput();
            GroundCheckRayCast();
            SpeedControl();
            JumpCheck();
            CrouchingPlayer();
            
        }


    }

    private void FixedUpdate()
    {
        if (pw.IsMine)
        //Animasyon icin geekli input islemleri;
        {
            PlayerMove();
            StateHandler();

        }
        

    }

    private void StateHandler()
    {
        //Kosma
        if(grounded && Input.GetKey(InputManager.sprintkey))
        {
            state = MovementState.SPRINTING;
            PlayerMoveSpeed = playerSprintSpeed;
            movementShake.GenerateImpulse();
            animator.SetBool("Crouching", false);
            animator.SetBool("Running", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Jump", false);
            
        }
        //yurume
        else if (grounded)
        {
            state = MovementState.WALK;
            PlayerMoveSpeed = playerWalkSpeed;
            animator.SetBool("Running", false);
            animator.SetBool("Crouching", false);
            animator.SetBool("Walking", true);
            animator.SetBool("Jump", false);

        }
        if (Input.GetKey(InputManager.crouchingKey))
        {
            state = MovementState.CROUCHING;
            PlayerMoveSpeed = crouchSpeed;
            animator.SetBool("Running", false);
            animator.SetBool("Crouching", true);
            animator.SetBool("Walking", true);
            animator.SetBool("Jump", false);
        }
        if(!grounded)
        {
            state = MovementState.AIR;
            animator.SetBool("Running",false);
            animator.SetBool("Crouching",false);
            animator.SetBool("Walking",false);
            animator.SetBool("Jump", true);
        }
    }

    private void PlayerMove()
    {
        //Animasyon icin gerekli bilgiler.
        animator.SetFloat("vinput", horizantalInput, 0.1f, Time.fixedDeltaTime);
        animator.SetFloat("hzinput", verticalInput, 0.1f, Time.fixedDeltaTime);
        //callculate movement direction

        horizantalInput = InputManager.HorizontalPos;
        verticalInput = InputManager.VerticalPos;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizantalInput;
        
        if (grounded)
        {
            //Yerdeyken uyguladigimiz hareket islemi Hava Surtunmesi olmadan.
            PlayerRb.AddForce(moveDirection.normalized * PlayerMoveSpeed * 10f, ForceMode.Force);
            
        }
        else if (!grounded)
        {
            //Eger yerde degil isek burada hava degiskeni ve gerekli animasyon islemleri devreye giriyor.

            PlayerRb.AddForce(moveDirection.normalized * PlayerMoveSpeed * 10f * airMultiplier, ForceMode.Force);
            
        }

    }

    private void GroundCheckRayCast()
    {
        //Karakterimizin ayaklari yere degiyor mu diye isin islemini kullaniyoruz.
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.4f + 0.2f, whatIsGround);

        if (grounded)
        {
            PlayerRb.drag = groundDrag;
        }
        else
        {
            //ayaklarim yere degmiyor.
            PlayerRb.drag = 0;
        }

    }
    private void SpeedControl()
    {
        //Karakterimizin durmadan hizlanmamasi icin yaptigimiz kontroller
        Vector3 flatVel = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);

        if (flatVel.magnitude > PlayerMoveSpeed)
        {
            Vector3 limetedVel = flatVel.normalized * PlayerMoveSpeed;
            PlayerRb.velocity = new Vector3(limetedVel.x, PlayerRb.velocity.y, limetedVel.z);
        }
    }
    private void Jump()
    {
        //Ziplama Methodu
        
        PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);
        PlayerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
    }
    private IEnumerator ResetJump()
    {
        //Animasyon icin ufak Time checkleri attiriyoruz ki animasyon daha iyi gozuksun ve burasi cooldown kismi
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Jump", false);

        yield return new WaitForSeconds(0.1f);
        readyToJump = true;
        
        
    }
    private void JumpCheck()
    {
        //Burada Ziplamaya hazir miyiz yere degiyor muyuz ve cooldown suresi dolmus mu kontrolleri
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            
            readyToJump = false;
            Jump();
            StartCoroutine(ResetJump());
            
        }
    }

    private void CrouchingPlayer()
    { 
        if (Input.GetKeyDown(InputManager.crouchingKey))
        {
            animator.SetBool("Crouching", true);
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            PlayerRb.AddForce(Vector3.down * 5f,ForceMode.Impulse);
        }
        if (Input.GetKeyDown(InputManager.crouchingKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            animator.SetBool("Crouching", false);


        }
    }
}
