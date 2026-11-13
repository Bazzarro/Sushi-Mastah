using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Animator animator;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // grab the model animator automatically if it was not assigned in the inspector
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // keep diagonal movement at the same speed as straight movement
        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        controller.SimpleMove(moveDirection * moveSpeed);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetFloat("Speed", moveDirection.magnitude);
    }
}
