using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public GameObject ball; // Particle effect to play upon impact (optional)

    public Transform point;

    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        // Create a new instance of the projectile
        GameObject projectile = Instantiate(ball, point.transform.position, Quaternion.identity);

        // Get the Rigidbody component of the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Check if the Rigidbody component exists
        if (rb != null)
        {
            // Set the velocity of the projectile
            rb.velocity = Camera.main.transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody component not found in the projectile prefab!");
        }
    }
}