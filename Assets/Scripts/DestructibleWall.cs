using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public GameObject prefabToInstantiate;  // Prefab to instantiate
    public int rows = 5;  // Number of rows
    public int columns = 5;  // Number of columns
    public float spacing = 1.0f;  // Spacing between prefabs

    void Start()
    {
        // Instantiate prefabs and set positions
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Instantiate prefab
                GameObject prefabInstance = Instantiate(prefabToInstantiate, transform);

                // Calculate position with spacing
                Vector3 position = new Vector3(col * spacing, row * spacing, 0);

                // Apply position to the instantiated prefab
                prefabInstance.transform.position = position;

                // Set parent to null to enable dynamic batching
                prefabInstance.transform.parent = null;
            }
        }
    }

    // Example method to destroy a prefab at a specific position
    public void DestroyPrefab(Vector3 position)
    {
        // Find the closest instantiated prefab to the given position
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DestructiblePrefab");
        GameObject closestPrefab = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject prefab in prefabs)
        {
            float distance = Vector3.Distance(prefab.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPrefab = prefab;
            }
        }

        // Destroy the closest prefab
        if (closestPrefab != null)
        {
            Destroy(closestPrefab);
        }
    }
}
