using UnityEngine;

public class KillzoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.Die();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.Die();
            }
        }
    }
}
