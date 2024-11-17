using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls projectile behavior, including movement and collision
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    // Initialize components
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Destroy the projectile if it goes out of bounds
    void Update()
    {
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }

    // Launch the projectile with a specific direction and force
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    // Handle collision logic
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.collider.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.TakeHit(); // Apply damage to the enemy
        }

        Destroy(gameObject); // Remove the projectile
    }
}
