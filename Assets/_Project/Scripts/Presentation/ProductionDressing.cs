using UnityEngine;

public sealed class ProductionDressing : MonoBehaviour
{
    [SerializeField] private GameObject[] props;
    [SerializeField] private Vector3[] positions;
    [SerializeField] private Vector3[] rotations;
    [SerializeField] private Vector3[] scales;
    [SerializeField] private bool disableColliders = true;

    private void Start()
    {
        // Gameplay_Level01 uses WorldChunk as its sole environment authority.
        // This presentation-only spawner remains valid for MainMenu, but must
        // not create non-colliding duplicate Military props in the game world.
        if (FindFirstObjectByType<InfiniteWorldController>() != null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (props == null)
            return;
        for (int i = 0; i < props.Length; i++)
        {
            GameObject prefab = props[i];
            if (prefab == null)
                continue;
            Vector3 position = positions != null && i < positions.Length ? positions[i] : transform.position;
            Quaternion rotation = rotations != null && i < rotations.Length ? Quaternion.Euler(rotations[i]) : Quaternion.identity;
            Vector3 scale = scales != null && i < scales.Length && scales[i] != Vector3.zero ? scales[i] : Vector3.one;
            GameObject instance = Instantiate(prefab, position, rotation, transform);
            instance.transform.localScale = scale;
            if (disableColliders)
            {
                foreach (Collider collider in instance.GetComponentsInChildren<Collider>(true))
                    collider.enabled = false;
            }
        }
    }
}
