using UnityEngine;

[RequireComponent(typeof(ZombieHealth))]
public sealed class LootDropper : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject medkitPrefab;
    [SerializeField, Range(0f, 1f)] private float coinDropChance = 1f;
    [SerializeField, Min(1)] private int minimumCoinValue = 1;
    [SerializeField, Min(1)] private int maximumCoinValue = 3;
    [SerializeField, Range(0f, 1f)] private float medkitDropChance = 0.1f;
    private bool dropped;
    private ZombieHealth health;
    private void Awake() { health = GetComponent<ZombieHealth>(); health.Died += Drop; }
    private void Drop()
    {
        if (dropped) return; dropped = true;
        Vector3 origin = transform.position + Vector3.up * 0.15f;
        if (coinPrefab != null && Random.value <= coinDropChance)
        {
            GameObject coin = Instantiate(coinPrefab, origin + new Vector3(Random.Range(-0.35f,0.35f),0f,Random.Range(-0.35f,0.35f)), Quaternion.identity);
            LootPickup pickup = coin.GetComponent<LootPickup>(); if (pickup != null) pickup.InitializeCoin(Random.Range(minimumCoinValue, maximumCoinValue + 1));
        }
        if (medkitPrefab != null && Random.value <= medkitDropChance)
            Instantiate(medkitPrefab, origin + new Vector3(Random.Range(-0.35f,0.35f),0.05f,Random.Range(-0.35f,0.35f)), Quaternion.identity);
    }
    private void OnDestroy() { if (health != null) health.Died -= Drop; }
}
