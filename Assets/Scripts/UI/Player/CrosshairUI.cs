using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Scaling Settings")]
    [SerializeField] private Vector3 normalScale = Vector3.one;
    [SerializeField] private Vector3 activeScale = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private float animationSpeed = 10f;

    private RectTransform crosshairRect;
    private Vector3 targetScale;

    private void Awake()
    {
        crosshairRect = GetComponent<RectTransform>();
        targetScale = normalScale;
    }

    private void OnEnable()
    {
        PlayerInteractionHandler.OnInteractableTargeted += SetActiveState;
    }

    private void OnDisable()
    {
        PlayerInteractionHandler.OnInteractableTargeted -= SetActiveState;
    }

    private void Update()
    {
        crosshairRect.localScale = Vector3.Lerp(crosshairRect.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    private void SetActiveState(bool isActive)
    {
        targetScale = isActive ? activeScale : normalScale;
    }
}
