using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    
    public Animator animator; // Drag your Animator component here in the Inspector

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Searches this object and its children (useful if the FBX mesh is nested)
    }

    void Update()
    {
        // 1. Read WASD / Arrow key input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // Normalize diagonal input so moving diagonally isn't faster
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
            print("should be moving");
        }

        // 2. Move the character
        controller.SimpleMove(moveDirection * moveSpeed);

        // 3. Smoothly rotate the character to face the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Send movement speed (0 to 1) to the Animator Controller
        animator.SetFloat("Speed", moveDirection.magnitude);
    }
}