using UnityEngine;

public class MeshInstancer : MonoBehaviour
{
    public GameObject prefabToRender;  // Prefab containing MeshFilter component(s)
    public Material material;  // Material to use for rendering
    public int instanceCount = 100;  // Number of instances to render

    ComputeBuffer matrixBuffer;  // Buffer to hold transformation matrices

    void Start()
    {
        // Extract mesh from the prefab
        Mesh meshToRender = ExtractMeshFromPrefab(prefabToRender);

        if (meshToRender == null)
        {
            Debug.LogError("Prefab does not contain a mesh.");
            return;
        }

        // Create a buffer to hold transformation matrices
        matrixBuffer = new ComputeBuffer(instanceCount, 16 * sizeof(float));

        // Generate random transformation matrices for instances
        Matrix4x4[] matrices = new Matrix4x4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            Quaternion rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            Vector3 scale = Vector3.one * Random.Range(0.5f, 2f);

            matrices[i] = Matrix4x4.TRS(position, rotation, scale);
        }

        // Set data in the buffer
        matrixBuffer.SetData(matrices);

        // Create a property block for the material
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        // Set the buffer to the property block
        propertyBlock.SetBuffer("_MatrixBuffer", matrixBuffer);

        // Draw mesh instances using instancing
        Graphics.DrawMeshInstanced(meshToRender, 0, material, matrices, instanceCount, propertyBlock, UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    void OnDestroy()
    {
        // Release the buffer
        if (matrixBuffer != null)
            matrixBuffer.Release();
    }

    Mesh ExtractMeshFromPrefab(GameObject prefab)
    {
        MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            return meshFilter.sharedMesh;
        }
        else
        {
            Debug.LogWarning("Prefab does not contain a MeshFilter component.");
            return null;
        }
    }
}
