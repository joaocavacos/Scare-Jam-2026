using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHits = 2;
    [SerializeField] private float timeBeforeRegen = 5.0f;
    
    private int currentHits;
    private float lastHitTime;
    private bool isDead = false;

    [Header("Damage UI Effect")]
    [SerializeField] private Image damageOverlay; 
    [SerializeField] private float damageAlpha = 0.5f;
    [SerializeField] private float uiFadeSpeed = 3f;

    private void Start()
    {
        currentHits = maxHits;

        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(1f, 0f, 0f, 0f); 
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        // Health Regeneration Logic
        if (currentHits < maxHits && Time.time >= lastHitTime + timeBeforeRegen)
        {
            RegenerateHealth();
        }

        HandleDamageUI();
    }

    public void TakeDamage()
    {
        if (isDead) return;

        currentHits--;
        lastHitTime = Time.time;

        if (currentHits <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log("Player took a hit!");
        }
    }

    private void RegenerateHealth()
    {
        currentHits = maxHits;
        Debug.Log("Player escaped and recovered full health.");
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("Player Died!");

        GameStateManager.Instance.TriggerGameOver();
    }

    private void HandleDamageUI()
    {
        if (damageOverlay == null)
        {
            return;
        }

        // If at 1 HP, fade to damageAlpha. If full HP, fade to 0 (invisible).
        float targetAlpha = (currentHits < maxHits) ? damageAlpha : 0f; 

        Color currentColor = damageOverlay.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * uiFadeSpeed);
        damageOverlay.color = currentColor;
    }
}
