using System;
using UnityEngine;

public class PlayerBreathController : MonoBehaviour
{
    [Header("Breath Settings")]
    [SerializeField, Tooltip("Maximum amount of seconds that the player can hold its breath")] 
    private float maxBreathTime = 5.0f;
    [SerializeField, Tooltip("The rate at which the player recovers its breath")] 
    private float recoveryRate = 2.0f;

    public static event Action<float> OnBreathUpdated;

    private PlayerInputHandler inputHandler;

    private float currentBreath;
    private bool isHoldingBreathInput = false;
    
    public bool IsHoldingBreath { get; private set; } 

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        currentBreath = maxBreathTime;
    }

    private void OnEnable()
    {
        inputHandler.OnHoldBreath += OnBreathInput;
    }

    private void OnDisable()
    {
        inputHandler.OnHoldBreath -= OnBreathInput;
    }

    private void OnBreathInput(bool isHolding)
    {
        isHoldingBreathInput = isHolding;
    }

    private void Update()
    {
        HandleBreathLogic();
    }

    private void HandleBreathLogic()
    {
        if (isHoldingBreathInput && currentBreath > 0)
        {
            // Drain breath
            IsHoldingBreath = true;
            currentBreath -= Time.deltaTime;

            // Force release if we run out of oxygen
            if (currentBreath <= 0)
            {
                currentBreath = 0;
                IsHoldingBreath = false;
                isHoldingBreathInput = false;

                // TODO: Play an audio for running out of breath
            }
        }
        else
        {
            // Recover breath
            IsHoldingBreath = false;
            if (currentBreath < maxBreathTime)
            {
                currentBreath += Time.deltaTime * recoveryRate;
                if (currentBreath > maxBreathTime)
                {
                    // Reached maximum breath value
                    currentBreath = maxBreathTime;
                }
            }
        }

        OnBreathUpdated?.Invoke(currentBreath / maxBreathTime);
    }
}
