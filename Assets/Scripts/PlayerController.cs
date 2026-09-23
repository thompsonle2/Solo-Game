using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

       /* // Lock mouse cursor to the center of the game screen and hide it
       replaced with crosshair cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; */
    }

    private void Update()
    {
        HandleMouseLook();
        HandleJump();
        HandleShooting();
    }

    private void FixedUpdate()
    {
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
        }
    }
}