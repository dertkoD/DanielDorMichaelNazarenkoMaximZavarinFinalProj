using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference reloadAction;

    [Header("Modules")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerJump playerJump;
    [SerializeField] private PlayerWeaponController playerWeaponController;
    [SerializeField] private PlayerAimController playerAimController;

    private void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.started += OnJumpStarted;
            jumpAction.action.canceled += OnJumpCanceled;
        }

        if (fireAction != null)
        {
            fireAction.action.Enable();
            fireAction.action.started += OnFireStarted;
            fireAction.action.canceled += OnFireCanceled;
        }

        if (reloadAction != null)
        {
            reloadAction.action.Enable();
            reloadAction.action.started += OnReloadStarted;
        }
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();

        if (jumpAction != null)
        {
            jumpAction.action.started -= OnJumpStarted;
            jumpAction.action.canceled -= OnJumpCanceled;
            jumpAction.action.Disable();
        }

        if (fireAction != null)
        {
            fireAction.action.started -= OnFireStarted;
            fireAction.action.canceled -= OnFireCanceled;
            fireAction.action.Disable();
        }

        if (reloadAction != null)
        {
            reloadAction.action.started -= OnReloadStarted;
            reloadAction.action.Disable();
        }
    }

    private void Update()
    {
        Vector2 moveInput = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        Vector2 lookInput = lookAction != null
            ? lookAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        playerMovement?.SetMoveInput(moveInput);
        playerAimController?.SetPointerScreenPosition(lookInput);
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        playerJump?.QueueJump();
        playerJump?.SetJumpHeld(true);
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        playerJump?.SetJumpHeld(false);
    }

    private void OnFireStarted(InputAction.CallbackContext context)
    {
        playerWeaponController?.StartFire();
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        playerWeaponController?.StopFire();
    }

    private void OnReloadStarted(InputAction.CallbackContext context)
    {
        playerWeaponController?.Reload();
    }
}
