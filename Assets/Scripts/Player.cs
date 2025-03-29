using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Vector2 playerSize = new Vector2(1f, 1f);

    [Header("State")]
    [SerializeField] private bool isGrounded;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public GameManager UI; 

    // References
    [SerializeField] private LayerMask platformLayerMask;

    // Visual effects
    [Header("Particle Effects")]
    public ParticleSystem fallParticle;
    public ParticleSystem bloodParticle;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landParticles;

    private bool wasGrounded;
    private PaintManager paintManager;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UI = UI.GetComponent<GameManager>();
        paintManager = FindObjectOfType<PaintManager>();

        isGrounded = true; // Start grounded
        wasGrounded = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f; // Use Unity's gravity
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        ProcessInput();

        if (!wasGrounded && isGrounded)
        {
            OnLanded();
        }
        wasGrounded = isGrounded;

        UpdateAnimations();
    }

    void FixedUpdate()
    {
        MovePlayer();
        CheckGrounded();
        ApplyPlatformEffects();
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(jumpForce);
        }
    }

    private void MovePlayer()
    {
        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.fixedDeltaTime);

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("Run", true);
            AudioManager.Instance.PlayWalkSound();

            //if(!AudioManager.Instance.walkSound.isPlaying) // Prevent restarting on every frame
            //{
            //    AudioManager.Instance.walkSound.loop = true; // Ensure it loops
            //    AudioManager.Instance.walkSound.Play();
            //    //dustParticle.Play();             
            //}
            //else
            //{
            //    AudioManager.Instance.walkSound.loop = false; // Stop looping
            //    AudioManager.Instance.walkSound.Stop();
            //}
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            animator.SetBool("Run", true);
            AudioManager.Instance.PlayWalkSound();
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    private void Jump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        AudioManager.Instance.PlayJumpSound();
        //if(!AudioManager.Instance.jumpSound.isPlaying) // Prevent restarting on every frame
        //    {
        //    AudioManager.Instance.jumpSound.loop = true; // Ensure it loops
        //    AudioManager.Instance.jumpSound.Play();
        //    //dustParticle.Play();             
        //}
        //else
        //{
        //    AudioManager.Instance.jumpSound.loop = false; // Stop looping
        //    AudioManager.Instance.jumpSound.Stop();
        //}

        if (jumpParticles != null)
        {
            jumpParticles.Play();
        }

        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        isGrounded = false;
    }

    private void OnLanded()
    {
        if (landParticles != null)
        {
            landParticles.Play();
        }

        if (animator != null)
        {
            animator.SetTrigger("Land");
        }
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            boxCollider.size * 0.95f,
            0f,
            Vector2.down,
            0.2f,
            platformLayerMask
        );

        // Check if grounded only when falling or stationary
        if (hit.collider != null && rb.linearVelocity.y <= 0f)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public PaintStroke GetPlatformUnder()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            boxCollider.size * 0.95f,
            0f,
            Vector2.down,
            0.2f,
            platformLayerMask
        );

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<PaintStroke>();
        }
        return null;
    }

    public void ApplyPlatformEffects()
    {
        PaintStroke platformUnder = GetPlatformUnder();

        if (platformUnder != null && isGrounded)
        {
            switch (platformUnder.PaintType)
            {
                case "blue":
                    Jump(platformUnder.BounceFactor);
                    break;

                case "yellow":
                    moveSpeed += platformUnder.SpeedBoost;
                    StartCoroutine(ResetSpeedAfterDelay(0.5f));
                    break;
            }
        }
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = 5f;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(150f, 100f, 0f);
        rb.linearVelocity = Vector2.zero;
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
            {
                isGrounded = true;
            }
        }
        
        if (collision.gameObject.CompareTag("gameOver") || collision.gameObject.CompareTag("obstacle"))
        {
            UI.GameOver();
        }

        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    ResetPosition();
        //}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
        {
            // Rely on CheckGrounded
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, playerSize);
    }
}