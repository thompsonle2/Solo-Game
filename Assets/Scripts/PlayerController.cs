using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Player Health")]
    public float playerHealth = 100f;

    //[Header("UI & Score")] // Added UI settings
   // [Tooltip("TextMeshPro component displaying the score.")]

    [Header("Movement")]
    [Tooltip("Movement speed (units/sec).")]
    public float speed = 5.0f;

    [Tooltip("Upward impulse force applied when jumping.")]
    public float jumpForce = 5.0f;

    [Tooltip("Layer mask indicating what surfaces count as ground.")]
    public LayerMask groundMask;

    [Header("Mouse Look")]
    [Tooltip("Reference to the child Camera Transform.")]
    public Transform playerCamera;

    [Tooltip("Sensitivity multiplier for mouse look.")]
    public float mouseSensitivity = 0.1f;

    [Tooltip("Minimum vertical angle limit (looking down).")]
    public float minXAngle = -80.0f;

    [Tooltip("Maximum vertical angle limit (looking up).")]
    public float maxXAngle = 80.0f;

    [Header("Combat Settings")]
    [Tooltip("Amount of damage dealt per shot hit.")]
    public float damage = 100f;

    [Tooltip("Maximum distance the hitscan raycast can travel.")]
    public float range = 100f;

    private Rigidbody rb;
    private float cameraPitch = 0.0f;
    private bool isGrounded;
    private bool isDead = false;

   // private int score = 0;
    //private int totalEnemies = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //hunt down gamemanager if not there
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    private void Update()
    {
        if (isDead) return;

        HandleMouseLook();
        HandleJump();
        HandleShooting();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        // Get mouse delta movement
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Horizontal look (Yaw): rotates player body on the Y-axis
        transform.Rotate(Vector3.up * mouseDelta.x);

        // Vertical look (Pitch): tilts camera on the X-axis
        cameraPitch -= mouseDelta.y;
        cameraPitch = Mathf.Clamp(cameraPitch, minXAngle, maxXAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;

        // Forward / Backward
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)   moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1f;

        // Strafe Left / Right
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)moveInput.x = 1f;

        // Normalize to prevent faster diagonal movement
        moveInput.Normalize();

        // Calculate velocity relative to player's facing direction
        Vector3 targetVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x) * speed;

        // Apply velocity, preserving existing gravity/jumping Y velocity
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void HandleJump()
    {
        // Simple raycast down to check if standing on ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundMask);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void HandleShooting()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (playerCamera == null) return;

        // Raycast forward from the camera center
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log($"Hit: {hit.collider.name}");

            // Check if the hit object has an Enemy component
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

        // Check for SideEnemy (Side-to-Side)
        SideEnemy sideEnemy = hit.collider.GetComponent<SideEnemy>();
        if (sideEnemy != null)
        {
            sideEnemy.TakeDamage(damage);
        }
        }
    }
    // update score when enemy dies
    /*public void AddScore(int amount = 1)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}/{totalEnemies}";
        }
    }*/

    // Detects collision with enemies
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Enemy" tag
        if (other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    // Also handles physical (non-trigger) collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("Player Died!");

            if (gameManager != null)
            {
                gameManager.gameOver();
            }
            else
            {
                Debug.LogError("GameManager is missing on PlayerController!");
            }

            // Unlock cursor so player can click Game Over screen buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

         //   gameObject.SetActive(false);
            // attempt to make camera go away when player dies
            rb.isKinematic = true;
            this.enabled = false; 
        }
    }
}