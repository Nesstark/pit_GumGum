using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float rollDistance = 1f; // Distance covered during the roll
    public float rollDuration = 0.3f; // Duration of the roll in seconds

    // Detection range
    public float detectionRadius = 10f; // Radius of the detection range

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private bool isRolling = false;
    private bool isInvulnerable = false;

    private Animator animator; // Reference to Animator

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // Get Animator component
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isRolling)
        {
            HandleMovement();
        }

        HandleLook();

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            StartCoroutine(PerformRoll());
        }
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y; // Preserve vertical movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Movement threshold to prevent small values keeping the isMoving state true
        float movementThreshold = 0.1f;
        bool isMoving = Mathf.Abs(curSpeedX) > movementThreshold || Mathf.Abs(curSpeedY) > movementThreshold;

        // Update animator based on movement
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }

    void HandleLook()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    IEnumerator PerformRoll()
    {
        isRolling = true;
        isInvulnerable = true; // Set player as invulnerable
        canMove = false;

        Vector3 rollDirection = moveDirection.normalized; // Roll in the current movement direction
        if (rollDirection == Vector3.zero)
        {
            rollDirection = transform.forward; // Default to forward direction if no movement
        }

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            characterController.Move(rollDirection * (rollDistance / rollDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
        isInvulnerable = false; // Reset invulnerability after the roll ends
        canMove = true;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    // Draw detection range gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
