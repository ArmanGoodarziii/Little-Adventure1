using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Mathematics;

public class Player : MonoBehaviourPun
{
    public InputSystem_Actions actions;
    private Rigidbody2D rb;
    private Animator animator;
    private float xScale; 
    [Header("Movement")]
    private float move;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJump;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask layerGround;

    private float faceDir = 1;
    private bool faceRight = true;
    private bool isGrounded;
    private bool isWall;
    private bool isWallJumping;
    

    void Awake()
    {
        actions = new InputSystem_Actions();
    }
    void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += Movement;
        actions.Player.Jump.performed += Jumping;

        actions.Player.Move.canceled += Movement;
        actions.Player.Jump.canceled += Jumping;
    }
    void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= Movement;
        actions.Player.Jump.performed -= Jumping;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        xScale = transform.localScale.x;
    }

    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine || GetComponent<PhotonView>() == null) return;
        
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, layerGround);
        isWall = Physics2D.Raycast(transform.position, Vector2.right * faceDir , wallCheckDistance, layerGround);
        
        HandleFlip();
        HandleAnimations();
    }
    void FixedUpdate()
    {
        if(!GetComponent<PhotonView>().IsMine) return;

        if (isWall)
        {
            rb.linearDamping = 8;
            return;
        }
        else
        {
            rb.linearDamping = 0;
        }
            
        if(isWallJumping) return;
        
        rb.linearVelocity = new Vector2(move * speed , rb.linearVelocity.y);
    }

    private void Movement(InputAction.CallbackContext ctx)
    {
        Vector2 vector2 = ctx.ReadValue<Vector2>();
        move = vector2.x;
    }
    private void Jumping(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x , jumpForce);
            }
             if (!isGrounded && isWall)
            {
                if (faceRight)
                {
                    faceRight = false;
                    faceDir = -1;
                    transform.localScale = new Vector3(-xScale, transform.localScale.y , transform.localScale.z);
                    rb.linearVelocity = new Vector2(-wallJump, jumpForce);
                }
                else if (!faceRight)
                {
                    faceRight = true;
                    faceDir = 1;
                    transform.localScale = new Vector3(xScale, transform.localScale.y , transform.localScale.z);
                    rb.linearVelocity = new Vector2(wallJump, jumpForce);
                }
                StopAllCoroutines();
                StartCoroutine(WallJumping());
            }
        }
    }
    private IEnumerator WallJumping()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(0.7f);
        isWallJumping = false;
    }
    
    private void HandleFlip()
    {
        if (move < -0.01)
        {
            faceRight = false;
            faceDir = -1;
            transform.localScale = new Vector3(-xScale , transform.localScale.y, transform.localScale.z);
        }
        else if (move > 0.01)
        {
            faceRight = true;
            faceDir = 1;
            transform.localScale = new Vector3(xScale , transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleAnimations()
    {
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWall", isWall);
    }

     

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position , new Vector2(transform.position.x , transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (faceDir * wallCheckDistance), transform.position.y));
    }
}
