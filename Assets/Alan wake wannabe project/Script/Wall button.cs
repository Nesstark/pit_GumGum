using UnityEngine;

public class WallButton : MonoBehaviour
{
    public GameObject wall;          // Reference to the wall GameObject
    public float moveDistance = 5f;  // How far the wall will move when the button is pressed
    public float moveSpeed = 2f;     // Speed at which the wall moves
    private bool isMovingUp = false; // Flag to track whether the wall is moving up or down
    private Vector3 initialPosition; // Starting position of the wall
    private Vector3 targetPosition;  // Target position of the wall

    private bool isPlayerNearby = false; // To check if the player is within range to press the button

    void Start()
    {
        if (wall != null)
        {
            initialPosition = wall.transform.position; // Get the initial position of the wall
            targetPosition = initialPosition + new Vector3(0, moveDistance, 0); // Set the target position
        }
        else
        {
            Debug.LogError("Wall reference is not assigned to the button.");
        }
    }

    void Update()
    {
        // If player is nearby and presses E, toggle the button action
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleWallMovement();
        }

        // If the wall is moving, move it
        if (wall != null)
        {
            if (isMovingUp && wall.transform.position.y < targetPosition.y)
            {
                MoveWall(targetPosition);
            }
            else if (!isMovingUp && wall.transform.position.y > initialPosition.y)
            {
                MoveWall(initialPosition);
            }
        }
    }

    // Toggle the wall movement between up and down
    void ToggleWallMovement()
    {
        isMovingUp = !isMovingUp; // Toggle the flag between moving up or down
        Debug.Log(isMovingUp ? "Wall moving up!" : "Wall moving down!");
    }

    // Move the wall to the target position smoothly
    void MoveWall(Vector3 target)
    {
        wall.transform.position = Vector3.MoveTowards(wall.transform.position, target, moveSpeed * Time.deltaTime);
    }

    // Detect when the player enters the button's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the object is the player
        {
            isPlayerNearby = true;
            Debug.Log("Press E to interact with the button.");
        }
    }

    // Detect when the player exits the button's collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
