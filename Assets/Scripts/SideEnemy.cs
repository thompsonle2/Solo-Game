using Vector3 = UnityEngine.Vector3;
using UnityEngine;

public class SideEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;

    [Header("Movement Settings (Side-to-Side)")]
    [Tooltip("Distance the enemy moves left and right along the X-axis.")]
    public float moveDistance = 3f;

    [Tooltip("Speed of the movement.")]
    public float speed = 2f;

    [Header("Effects (Optional)")]
    public GameObject deathEffect;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Moves side-to-side along the X-axis
        float offset = Mathf.Sin(Time.time * speed) * moveDistance;
        transform.position = startPosition + new Vector3(offset, 0f, 0f);
    }

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
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(1);
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}