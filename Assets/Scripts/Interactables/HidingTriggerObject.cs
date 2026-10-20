using System;
using UnityEngine;

public class HidingTriggerObject : MonoBehaviour, IInteractable
{
    [Header("Location Transforms")]
    [SerializeField] private Transform hidingPlaceTransform;
    [SerializeField] private Transform exitPlaceTransform;

    public static event Action<Transform, Transform> OnHidingTriggered;

    public void Interact()
    {
        OnHidingTriggered?.Invoke(hidingPlaceTransform, exitPlaceTransform);
    }
}
