using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;

    [Header("Effects (Optional)")]
    [Tooltip("Optional visual effect prefab to spawn when destroyed.")]
    public GameObject deathEffect;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Spawn particle effect if assigned
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
}