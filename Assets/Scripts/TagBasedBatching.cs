using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TagBasedBatching : MonoBehaviour
{
    public string[] tagsToBatch = new string[18 * 3 + 1] ;
    public Material sharedMaterial;

    GameObject combinedObject = null;

    bool isVisible = false;

    void Start()
    {
        // StartCoroutine(ToggleVisibilityCoroutine());
        tagsToBatch[0] = "static";
        
        int index = 1;
        for (int i = 1; i <= 18; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                tagsToBatch[index] = $"{j}_{i}";
                index++;
            }
        }

        foreach (var tag in tagsToBatch)
        {
            CombineMeshesWithTag(tag);
        }
    }

    void CombineMeshesWithTag(string tag)
    {
        GameObject[] objectsToCombine = GameObject.FindGameObjectsWithTag(tag);
        if (objectsToCombine.Length == 0) return;

        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<Collider> originalColliders = new List<Collider>();

        foreach (var obj in objectsToCombine)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Collider collider = obj.GetComponent<Collider>();
            if (meshFilter != null)
            {
                meshFilters.Add(meshFilter);
            }

            if (collider != null)
            {
                originalColliders.Add(collider);
            }
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        

        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Disable original object
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine, true, true);

        combinedObject = new GameObject($"Combined Mesh {tag}");
        combinedObject.AddComponent<MeshFilter>().mesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>().material = sharedMaterial;
        
        Rigidbody rigidbody = combinedObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = true;

        
        Bounds combinedBounds = CalculateCombinedBounds(originalColliders);

        GameObject compoundColliderObject = new GameObject("CompoundCollider");
        compoundColliderObject.transform.parent = combinedObject.transform;

        MeshCollider compoundCollider = compoundColliderObject.AddComponent<MeshCollider>();

        compoundCollider.convex = true;
        compoundCollider.sharedMesh = combinedMesh;

        
        foreach (Collider collider in originalColliders)
        {
            collider.enabled = false;
        }

        // Set the combined object to receive and cast shadows
        
        combinedObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        combinedObject.GetComponent<MeshRenderer>().receiveShadows = true;

        // Calculate bounds for culling
        combinedMesh.RecalculateBounds();
    }

    IEnumerator ToggleVisibilityCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Wait for 2 seconds

            // Toggle visibility
            isVisible = !isVisible;
            combinedObject.SetActive(isVisible);
        }
    }

        // Calculate the bounds that encompass all child colliders
    private Bounds CalculateCombinedBounds(List<Collider> colliders)
    {
        Bounds combinedBounds = new Bounds();
        foreach (Collider collider in colliders)
        {
            combinedBounds.Encapsulate(collider.bounds);
        }
        return combinedBounds;
    }
}
