using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Sensitivity")]
    [SerializeField] private bool useDebugSensitivity = true;
    [SerializeField] private float debugLookSensitivity = 0.15f;

    [Header("Limits")]
    [SerializeField] private float topClamp = -89.0f;
    [SerializeField] private float bottomClamp = 89.0f;

    [Header("Headbob Settings")]
    [SerializeField] private float bobFrequency = 12f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    private PlayerInputHandler inputHandler;
    private CharacterController characterController;
    private float cameraPitch = 0.0f;

    // Headbob tracking
    private float defaultYPosition = 0f;
    private float timer = 0f;
    private Vector3 lastPosition;

    // Sensitivity
    private float currentSensitivity;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (playerCamera != null)
        {
            defaultYPosition = playerCamera.localPosition.y;
        }

        lastPosition = transform.position;

        currentSensitivity = useDebugSensitivity ? debugLookSensitivity : PlayerPrefs.GetFloat("MouseSensitivity", debugLookSensitivity);

        // TODO: Remove after game state management is implemented
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        ApplyCameraLook();
        ApplyHeadbob();
    }

    private void ApplyCameraLook()
    {
        Vector2 lookInput = inputHandler.LookInput * currentSensitivity;

        // Calculate pitch, we subtract the Y input so pushing the mouse up looks up (standard non-inverted)
        cameraPitch -= lookInput.y;
        cameraPitch = Mathf.Clamp(cameraPitch, topClamp, bottomClamp);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Calculate Yaw and apply directly to the Player's body so forward movement always matches where you look
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void ApplyHeadbob()
    {
        // Calculate actual physical distance moved this frame (ignoring Y/vertical axis)
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 previousPos = new Vector3(lastPosition.x, 0, lastPosition.z);
    
        float speed = Vector3.Distance(currentPos, previousPos) / Time.deltaTime;
    
        // Update last position for the next frame
        lastPosition = transform.position;

        if (speed > 0.1f)
        {
            // Advance the sine wave timer based on speed.
            timer += Time.deltaTime * bobFrequency * (speed / 3.0f);

            // Increase amplitude if moving faster than walking
            float currentAmplitude = (speed > 4.0f) ? bobAmplitude * sprintMultiplier : bobAmplitude;

            // Calculate the new Y position
            float newY = defaultYPosition + Mathf.Sin(timer) * currentAmplitude;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, newY, playerCamera.localPosition.z);
        }
        else
        {
            // Smoothly reset the camera to the center when stopped
            timer = 0;
            float resetY = Mathf.Lerp(playerCamera.localPosition.y, defaultYPosition, Time.deltaTime * 5f);
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, resetY, playerCamera.localPosition.z);
        }
    }
}
