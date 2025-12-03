using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb; 
    private float xScale;

    [Header("Movement")]
    [SerializeField] private float speed;

    [Header("Collision")]
    [SerializeField] private float checkGroundDistance;
    [SerializeField] private float checkWallDistance;
    [SerializeField] private Transform checkNoGroundTransform;
    [SerializeField] private LayerMask layerGround;

    private bool isGrounded;
    private bool checkNoGround;
    private bool isWall;

    private bool faceRight = false;
    private float faceDir = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        xScale = transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed * faceDir , rb.linearVelocity.y);

        HandleCollision();
        HandleFlip();
    }
    private void HandleFlip()
    {
        if (isGrounded)
        {
            if(isWall || !checkNoGround)
            {
                faceRight = !faceRight;
            }
        }

        if (faceRight)
        {
            faceDir = 1;
            transform.localScale = new Vector3(-xScale, transform.localScale.y, transform.localScale.z);
        }
        else if (!faceRight)
        {
            faceDir = -1;
            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        }
    }
    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position , Vector2.down, checkGroundDistance, layerGround);
        checkNoGround = Physics2D.Raycast(checkNoGroundTransform.position, Vector2.down, checkGroundDistance, layerGround);
        isWall = Physics2D.Raycast(transform.position , Vector2.right * faceDir, checkWallDistance, layerGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x , transform.position.y - checkGroundDistance));
        Gizmos.DrawLine(checkNoGroundTransform.position , new Vector2(checkNoGroundTransform.position.x , checkNoGroundTransform.position.y - checkGroundDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (faceDir * checkWallDistance), transform.position.y));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
            collision.GetComponent<Player>().Hit();
    }
}
