using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public MeshFilter[] meshesToCombine;
    public Material combinedMaterial;
    public float spacing = 1.0f; // Adjust as needed
    // Start is called before the first frame update

    [SerializeField] DrawCombinedMesh drawCombinedMesh;
    void Start()
    {
        // Prepare arrays for CombineMeshes
        CombineInstance[] combineInstances = new CombineInstance[meshesToCombine.Length];

        drawCombinedMesh.Example();

        for (int i = 0; i < meshesToCombine.Length; i++)
        {
            // Calculate the translation component of the transformation matrix
            Vector3 positionOffset = Camera.main.transform.forward * i * spacing;

            // Create a transformation matrix with the translation component
            Matrix4x4 matrix = Matrix4x4.TRS(positionOffset, Quaternion.identity, Vector3.one);

            combineInstances[i].transform = matrix * meshesToCombine[i].transform.localToWorldMatrix;
        }

        // Create a new GameObject to hold the combined mesh
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.position = Vector3.zero;

        // Add MeshFilter and MeshRenderer components
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, false);

        // Assign material
        if (combinedMaterial != null)
        {
            meshRenderer.sharedMaterial = combinedMaterial;
        }

        // Assign combined mesh to MeshFilter
        meshFilter.sharedMesh = combinedMesh;
    }
}
