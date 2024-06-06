using UnityEngine;

public class PrefabBatcher : MonoBehaviour
{
    public GameObject blockPrefab;  // Prefab for the block
    public int rows = 5;  // Number of rows
    public int columns = 5;  // Number of columns
    public float spacing = 1.0f;  // Spacing between blocks

    void Start()
    {
        // Prepare arrays for CombineMeshes
        CombineInstance[] combineInstances = new CombineInstance[rows * columns];

        // Loop to instantiate blocks and set positions
        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject block = Instantiate(blockPrefab, transform);
                Vector3 position = new Vector3(col * spacing, row * spacing, 0);
                block.transform.position = position;
                combineInstances[index].mesh = block.GetComponent<MeshFilter>().sharedMesh;
                combineInstances[index].transform = block.GetComponent<MeshFilter>().transform.localToWorldMatrix;

                // Increment index
                index++;
            }
        }

        // Create a new GameObject to hold the combined mesh
        GameObject combinedObject = new GameObject("CombinedWall");
        combinedObject.transform.position = Vector3.zero;

        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, false);
        meshFilter.sharedMesh = combinedMesh;

        meshRenderer.additionalVertexStreams = combinedMesh;

        if (blockPrefab != null)
        {
            meshRenderer.sharedMaterial = blockPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}
