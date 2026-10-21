using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public event Action OnInteract;
    public event Action OnSnapGlowstick;
    public event Action<bool> OnToggleCrouch;
    public event Action<bool> OnToggleSprint;
    public event Action<bool> OnHoldBreath;

    private bool isCrouched = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        inputActions.Player.Crouch.performed += HandleCrouch;

        inputActions.Player.Sprint.started += HandleSprint;
        inputActions.Player.Sprint.canceled += HandleSprint;

        inputActions.Player.Interact.performed += HandleInteract;

        inputActions.Player.HoldBreath.performed += HandleBreath;
        inputActions.Player.HoldBreath.canceled += HandleBreath;

        inputActions.Player.Glowstick.performed += ctx => OnSnapGlowstick?.Invoke();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.Move.performed -= ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => MoveInput = Vector2.zero;

        inputActions.Player.Look.performed -= ctx => LookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled -= ctx => LookInput = Vector2.zero;

        inputActions.Player.Crouch.performed -= HandleCrouch;

        inputActions.Player.Sprint.started -= HandleSprint;
        inputActions.Player.Sprint.canceled -= HandleSprint;

        inputActions.Player.Interact.performed -= HandleInteract;
        
        inputActions.Player.HoldBreath.performed -= HandleBreath;
        inputActions.Player.HoldBreath.canceled -= HandleBreath;

        inputActions.Player.Glowstick.performed -= ctx => OnSnapGlowstick?.Invoke();
    }

    private void HandleCrouch(InputAction.CallbackContext context)
    {
        isCrouched = !isCrouched;
        OnToggleCrouch?.Invoke(isCrouched);
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnToggleSprint?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnToggleSprint?.Invoke(false);
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }
    
    private void HandleBreath(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnHoldBreath?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnHoldBreath?.Invoke(false);
        }
    }
}
