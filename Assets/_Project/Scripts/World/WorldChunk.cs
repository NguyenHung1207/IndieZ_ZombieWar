using UnityEngine;

public sealed class WorldChunk : MonoBehaviour
{
    public Vector2Int Coordinate { get; private set; }
    private float size;
    private Material groundMaterial;
    private GameObject[] props;

    public void Configure(Vector2Int coordinate, float chunkSize, Material material, GameObject[] propPrefabs, int layout)
    {
        Coordinate = coordinate;
        size = chunkSize;
        groundMaterial = material;
        props = propPrefabs;
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        transform.position = new Vector3(coordinate.x * size, 0f, coordinate.y * size);
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ChunkGround";
        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = new Vector3(size * 0.5f, -0.12f, size * 0.5f);
        floor.transform.localScale = new Vector3(size, 0.2f, size);
        if (groundMaterial != null)
            floor.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        ApplyLayout(layout);
    }

    private void ApplyLayout(int layout)
    {
        switch (layout % 6)
        {
            case 0: Place(1, 0.28f, 0.25f); Place(2, -0.25f, -0.2f); Place(3, 0.28f, -0.28f); break;
            case 1: Place(0, -0.28f, 0.18f); Place(0, -0.28f, -0.18f); Place(8, 0.22f, 0f); Place(2, 0.30f, -0.25f); break;
            case 2: Place(4, -0.25f, 0.2f); Place(5, 0.25f, 0.15f); Place(1, 0.2f, -0.25f); Place(2, -0.25f, -0.25f); break;
            case 3: Place(6, 0.18f, 0.18f); Place(1, -0.22f, 0.22f); Place(2, -0.25f, -0.25f); break;
            case 4: Place(0, -0.28f, -0.2f); Place(7, 0.22f, 0.2f); Place(1, 0.22f, -0.28f); Place(3, -0.2f, 0.28f); break;
            default: Place(7, 0f, 0f); Place(1, 0.3f, -0.25f); Place(2, -0.28f, 0.25f); break;
        }
    }

    private void Place(int index, float x, float z)
    {
        if (props == null || index < 0 || index >= props.Length || props[index] == null)
            return;
        GameObject instance = Instantiate(props[index], transform);
        instance.transform.localPosition = new Vector3(x * size, 0f, z * size);
        instance.transform.localRotation = Quaternion.Euler(0f, (Coordinate.x * 37 + Coordinate.y * 19) % 4 * 90f, 0f);
        instance.transform.localScale = Vector3.one;
        foreach (MeshCollider mesh in instance.GetComponentsInChildren<MeshCollider>(true))
            mesh.enabled = false;
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            BoxCollider box = instance.AddComponent<BoxCollider>();
            box.center = instance.transform.InverseTransformPoint(bounds.center);
            box.size = bounds.size;
        }
    }
}
