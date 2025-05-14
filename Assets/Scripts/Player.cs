////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;

////public class PlayerController : MonoBehaviour
////{
////    [Header("Movement Settings")]
////    [SerializeField] private float moveSpeed = 5f;
////    [SerializeField] private float jumpForce = 10f;
////    [SerializeField] private Vector2 playerSize = new Vector2(1f, 1f);

////    [Header("State")]
////    [SerializeField] private bool isGrounded;

////    private Rigidbody2D rb;
////    private BoxCollider2D boxCollider;
////    private SpriteRenderer spriteRenderer;
////    private Animator animator;
////    public GameManager UI;

////    // References
////    [SerializeField] private LayerMask platformLayerMask;

////    // Visual effects
////    [Header("Particle Effects")]
////    public ParticleSystem fallParticle;
////    public ParticleSystem bloodParticle;
////    [SerializeField] private ParticleSystem jumpParticles;
////    [SerializeField] private ParticleSystem landParticles;

////    private bool wasGrounded;
////    private PaintManager paintManager;
////    private float horizontalInput;
////    private bool isJumping; // Track if jump animation is active

////    void Start()
////    {
////        rb = GetComponent<Rigidbody2D>();
////        boxCollider = GetComponent<BoxCollider2D>();
////        spriteRenderer = GetComponent<SpriteRenderer>();
////        animator = GetComponent<Animator>();
////        UI = UI.GetComponent<GameManager>();
////        paintManager = FindObjectOfType<PaintManager>();

////        isGrounded = true;
////        wasGrounded = false;
////        isJumping = false;

////        rb.bodyType = RigidbodyType2D.Dynamic;
////        rb.gravityScale = 1f;
////        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
////    }

////    void Update()
////    {
////        ProcessInput();

////        if (!wasGrounded && isGrounded)
////        {
////            OnLanded();
////        }
////        wasGrounded = isGrounded;

////        UpdateAnimations();
////    }

////    void FixedUpdate()
////    {
////        MovePlayer();
////        CheckGrounded();
////        ApplyPlatformEffects();
////    }

////    private void ProcessInput()
////    {
////        horizontalInput = Input.GetAxisRaw("Horizontal");

////        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
////        {
////            Jump(jumpForce);
////        }
////    }

////    private void MovePlayer()
////    {
////        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.fixedDeltaTime);

////        if (horizontalInput > 0)
////        {
////            transform.localScale = new Vector3(1, 1, 1);
////            AudioManager.Instance.PlayWalkSound();
////        }
////        else if (horizontalInput < 0)
////        {
////            transform.localScale = new Vector3(-1, 1, 1);
////            AudioManager.Instance.PlayWalkSound();
////        }
////    }

////    private void Jump(float force)
////    {
////        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
////        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
////        AudioManager.Instance.PlayJumpSound();

////        if (jumpParticles != null)
////        {
////            jumpParticles.Play();
////        }

////        if (animator != null)
////        {
////            animator.SetTrigger("Jump");
////            isJumping = true; // Lock into jump state
////        }

////        isGrounded = false;
////    }

////    private void OnLanded()
////    {
////        if (landParticles != null)
////        {
////            landParticles.Play();
////        }

////        if (animator != null)
////        {
////            animator.SetTrigger("Land");
////            isJumping = false; // Exit jump state on landing
////        }
////    }

////    private void UpdateAnimations()
////    {
////        if (animator != null)
////        {
////            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
////            animator.SetBool("IsGrounded", isGrounded);

////            // Only update run animation if grounded and not jumping
////            if (isGrounded && !isJumping)
////            {
////                animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
////                animator.SetBool("Run", Mathf.Abs(horizontalInput) > 0);
////            }
////            else
////            {
////                animator.SetBool("Run", false); // Disable run animation while airborne
////            }
////        }
////    }

////    private void CheckGrounded()
////    {
////        RaycastHit2D hit = Physics2D.BoxCast(
////            transform.position,
////            boxCollider.size * 0.95f,
////            0f,
////            Vector2.down,
////            0.2f,
////            platformLayerMask
////        );

////        if (hit.collider != null && rb.linearVelocity.y <= 0f)
////        {
////            isGrounded = true;
////        }
////        else
////        {
////            isGrounded = false;
////        }
////    }

////    public PaintStroke GetPlatformUnder()
////    {
////        RaycastHit2D hit = Physics2D.BoxCast(
////            transform.position,
////            boxCollider.size * 0.95f,
////            0f,
////            Vector2.down,
////            0.2f,
////            platformLayerMask
////        );

////        if (hit.collider != null)
////        {
////            return hit.collider.GetComponent<PaintStroke>();
////        }
////        return null;
////    }

////    public void ApplyPlatformEffects()
////    {
////        PaintStroke platformUnder = GetPlatformUnder();

////        if (platformUnder != null && isGrounded)
////        {
////            switch (platformUnder.PaintType)
////            {
////                case "blue":
////                    Jump(platformUnder.BounceFactor);
////                    break;

////                case "yellow":
////                    moveSpeed += platformUnder.SpeedBoost;
////                    StartCoroutine(ResetSpeedAfterDelay(0.5f));
////                    break;
////            }
////        }
////    }

////    private IEnumerator ResetSpeedAfterDelay(float delay)
////    {
////        yield return new WaitForSeconds(delay);
////        moveSpeed = 5f;
////    }

////    public void ResetPosition()
////    {
////        transform.position = new Vector3(150f, 100f, 0f);
////        rb.linearVelocity = Vector2.zero;
////        isGrounded = false;
////        isJumping = false;
////    }

////    private void OnCollisionEnter2D(Collision2D collision)
////    {
////        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
////        {
////            ContactPoint2D contact = collision.GetContact(0);
////            if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
////            {
////                isGrounded = true;
////            }
////        }

////        if (collision.gameObject.CompareTag("gameOver") || collision.gameObject.CompareTag("obstacle"))
////        {
////            UI.GameOver();
////        }
////    }

////    private void OnCollisionExit2D(Collision2D collision)
////    {
////        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
////        {
////            // Rely on CheckGrounded
////        }
////    }

////    private void OnDrawGizmosSelected()
////    {
////        Gizmos.color = Color.blue;
////        Gizmos.DrawWireCube(transform.position, playerSize);
////    }
////}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    [Header("Movement Settings")]
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private float jumpForce = 10f;
//    [SerializeField] private Vector2 playerSize = new Vector2(1f, 1f);

//    [Header("State")]
//    [SerializeField] private bool isGrounded;

//    private Rigidbody2D rb;
//    private BoxCollider2D boxCollider;
//    private SpriteRenderer spriteRenderer;
//    private Animator animator;
//    public GameManager UI;

//    // References
//    [SerializeField] private LayerMask platformLayerMask;

//    // Visual effects
//    [Header("Particle Effects")]
//    public ParticleSystem fallParticle;
//    public ParticleSystem bloodParticle;
//    [SerializeField] private ParticleSystem jumpParticles;
//    [SerializeField] private ParticleSystem landParticles;

//    private bool wasGrounded;
//    private PaintManager paintManager;
//    private float horizontalInput;
//    private bool isJumping;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        boxCollider = GetComponent<BoxCollider2D>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        animator = GetComponent<Animator>();
//        UI = UI.GetComponent<GameManager>();
//        paintManager = FindObjectOfType<PaintManager>();

//        isGrounded = true;
//        wasGrounded = false;
//        isJumping = false;

//        rb.bodyType = RigidbodyType2D.Dynamic;
//        rb.gravityScale = 1f;
//        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//    }

//    void Update()
//    {
//        ProcessInput();

//        if (!wasGrounded && isGrounded)
//        {
//            OnLanded();
//        }
//        wasGrounded = isGrounded;

//        UpdateAnimations();
//    }

//    void FixedUpdate()
//    {
//        MovePlayer();
//        CheckGrounded();
//        ApplyPlatformEffects();
//    }

//    private void ProcessInput()
//    {
//        horizontalInput = Input.GetAxisRaw("Horizontal");

//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
//        {
//            Jump(jumpForce);
//        }
//    }

//    private void MovePlayer()
//    {
//        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.fixedDeltaTime);

//        if (horizontalInput > 0)
//        {
//            transform.localScale = new Vector3(1, 1, 1);
//            AudioManager.Instance.PlayWalkSound();
//        }
//        else if (horizontalInput < 0)
//        {
//            transform.localScale = new Vector3(-1, 1, 1);
//            AudioManager.Instance.PlayWalkSound();
//        }
//    }

//    private void Jump(float force)
//    {
//        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
//        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
//        AudioManager.Instance.PlayJumpSound();

//        if (jumpParticles != null)
//        {
//            jumpParticles.Play();
//        }

//        if (animator != null)
//        {
//            animator.SetTrigger("Jump");
//            isJumping = true;
//        }

//        isGrounded = false;
//    }

//    private void OnLanded()
//    {
//        if (landParticles != null)
//        {
//            landParticles.Play();
//        }

//        if (animator != null)
//        {
//            animator.SetTrigger("Land");
//            isJumping = false;
//        }
//    }

//    private void UpdateAnimations()
//    {
//        if (animator != null)
//        {
//            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
//            animator.SetBool("IsGrounded", isGrounded);

//            PaintStroke platformUnder = GetPlatformUnder();
//            bool isOnYellowPaint = platformUnder != null && platformUnder.PaintType == "yellow" && isGrounded;

//            if (isGrounded && !isJumping)
//            {
//                animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
//                animator.SetBool("Run", Mathf.Abs(horizontalInput) > 0 && !isOnYellowPaint);
//                animator.SetBool("FastRun", Mathf.Abs(horizontalInput) > 0 && isOnYellowPaint);
//            }
//            else
//            {
//                animator.SetBool("Run", false);
//                animator.SetBool("FastRun", false);
//            }
//        }
//    }

//    private void CheckGrounded()
//    {
//        RaycastHit2D hit = Physics2D.BoxCast(
//            transform.position,
//            boxCollider.size * 0.95f,
//            0f,
//            Vector2.down,
//            0.2f,
//            platformLayerMask
//        );

//        if (hit.collider != null && rb.linearVelocity.y <= 0f)
//        {
//            isGrounded = true;
//        }
//        else
//        {
//            isGrounded = false;
//        }
//    }

//    public PaintStroke GetPlatformUnder()
//    {
//        RaycastHit2D hit = Physics2D.BoxCast(
//            transform.position,
//            boxCollider.size * 0.95f,
//            0f,
//            Vector2.down,
//            0.2f,
//            platformLayerMask
//        );

//        if (hit.collider != null)
//        {
//            return hit.collider.GetComponent<PaintStroke>();
//        }
//        return null;
//    }

//    public void ApplyPlatformEffects()
//    {
//        PaintStroke platformUnder = GetPlatformUnder();

//        if (platformUnder != null && isGrounded)
//        {
//            switch (platformUnder.PaintType)
//            {
//                case "blue":
//                    Jump(platformUnder.BounceFactor);
//                    break;

//                case "yellow":
//                    moveSpeed += platformUnder.SpeedBoost;
//                    StartCoroutine(ResetSpeedAfterDelay(0.5f));
//                    break;
//            }
//        }
//    }

//    private IEnumerator ResetSpeedAfterDelay(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        moveSpeed = 5f;
//    }

//    public void ResetPosition()
//    {
//        transform.position = new Vector3(150f, 100f, 0f);
//        rb.linearVelocity = Vector2.zero;
//        isGrounded = false;
//        isJumping = false;
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
//        {
//            ContactPoint2D contact = collision.GetContact(0);
//            if (contact.normal.y > 0.5f && rb.linearVelocity.y <= 0f)
//            {
//                isGrounded = true;
//            }
//        }

//        if (collision.gameObject.CompareTag("gameOver") || collision.gameObject.CompareTag("obstacle"))
//        {
//            UI.GameOver();
//        }
//    }

//    private void OnCollisionExit2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Paint"))
//        {
//            // Rely on CheckGrounded
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireCube(transform.position, playerSize);
//    }
//}


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
    private bool isJumping; // Now controls both animation and blue paint logic

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UI = UI.GetComponent<GameManager>();
        paintManager = FindObjectOfType<PaintManager>();

        isGrounded = true;
        wasGrounded = false;
        isJumping = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            StartJump(jumpForce);
        }
    }

    private void MovePlayer()
    {
        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.fixedDeltaTime);

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            AudioManager.Instance.PlayWalkSound();
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            AudioManager.Instance.PlayWalkSound();
        }
    }

    private void StartJump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        AudioManager.Instance.PlayJumpSound();

        if (jumpParticles != null)
        {
            jumpParticles.Play();
        }

        isJumping = true; // Set jumping state
        isGrounded = false;
    }

    private void OnLanded()
    {
        if (landParticles != null)
        {
            landParticles.Play();
        }

        isJumping = false; // End jumping state unless blue paint triggers it again
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("Jump", isJumping); // Use bool for jump state

            PaintStroke platformUnder = GetPlatformUnder();
            bool isOnYellowPaint = platformUnder != null && platformUnder.PaintType == "yellow" && isGrounded;

            if (isGrounded && !isJumping)
            {
                animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
                animator.SetBool("Run", Mathf.Abs(horizontalInput) > 0 && !isOnYellowPaint);
                animator.SetBool("FastRun", Mathf.Abs(horizontalInput) > 0 && isOnYellowPaint);
            }
            else
            {
                animator.SetBool("Run", false);
                animator.SetBool("FastRun", false);
            }
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

        if (platformUnder != null && isGrounded && !isJumping)
        {
            switch (platformUnder.PaintType)
            {
                case "blue":
                    StartJump(platformUnder.BounceFactor); // Auto-jump on blue paint
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
        isJumping = false;
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