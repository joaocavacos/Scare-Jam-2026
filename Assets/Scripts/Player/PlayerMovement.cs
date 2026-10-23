using ArtificeToolkit.Attributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Values")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 2.0f;

    private CharacterController controller;

    private Vector3 velocity;
    [SerializeField, ReadOnly] private bool isSprinting = false;
    [SerializeField, ReadOnly] private bool isCrouching = false;

    public Transform GetPlayerTransform()
    {
        return transform;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnToggleSprint += HandleSprint;
        PlayerInputHandler.OnToggleCrouch += HandleCrouch;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnToggleSprint -= HandleSprint;
        PlayerInputHandler.OnToggleCrouch -= HandleCrouch;
    }

    private void Update()
    {
        ApplyMovement();
        ApplyGravity();
    }

    private void ApplyMovement()
    {
        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isSprinting) currentSpeed = sprintSpeed;

        Vector2 input = PlayerInputHandler.Instance.MoveInput;

        if(input.sqrMagnitude > 0f)
        {
            Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
            controller.Move(moveDirection * (currentSpeed * Time.deltaTime));
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            // Keep the player grounded
            velocity.y = -2f;
        }

        // Apply gravity over time
        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleSprint(bool InIsSprinting)
    {
        if (isCrouching)
        {
            return;
        }

        isSprinting = InIsSprinting;
    }

    private void HandleCrouch(bool InIsCrouching)
    {
        isCrouching = InIsCrouching;

        if(isCrouching)
        {
            isSprinting = false;
        }
    }
}
