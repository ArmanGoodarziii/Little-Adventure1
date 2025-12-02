using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviourPun
{
    public InputSystem_Actions actions;
    private PhotonView pv;
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

    public GameObject cameraObject;
    public GameObject canvasObject;
    public float yPos;

    private float faceDir = 1;
    private bool faceRight = true;
    private bool isGrounded;
    private bool isWall;
    private bool isWallJumping;
    

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            cameraObject.SetActive(true);
            actions = new InputSystem_Actions();
        }
            
    }
    void OnEnable()
    {
        if (pv.IsMine)
        {
            actions.Player.Enable();
            actions.Player.Move.performed += Movement;
            actions.Player.Jump.performed += Jumping;

            actions.Player.Move.canceled += Movement;
            actions.Player.Jump.canceled += Jumping;
        }
    }
    void OnDisable()
    {
        if (pv.IsMine)
        {
            actions.Player.Disable();
            actions.Player.Move.performed -= Movement;
            actions.Player.Jump.performed -= Jumping;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        xScale = transform.localScale.x;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            HandleCollision();
            HandleFlip();
            HandleAnimations();

            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, new Vector3(transform.position.x, transform.position.y, cameraObject.transform.position.z), Time.deltaTime);
        }
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
    void LateUpdate()
    {
        canvasObject.transform.position = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z);
    }

    private void Movement(InputAction.CallbackContext ctx)
    {
        if(!pv.IsMine) return;

        Vector2 vector2 = ctx.ReadValue<Vector2>();
        move = vector2.x;
    }
    private void Jumping(InputAction.CallbackContext ctx)
    {
        if(!pv.IsMine) return;

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

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, layerGround);
        isWall = Physics2D.Raycast(transform.position, Vector2.right * faceDir , wallCheckDistance, layerGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position , new Vector2(transform.position.x , transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (faceDir * wallCheckDistance), transform.position.y));
    }
}
