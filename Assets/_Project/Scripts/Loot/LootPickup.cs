using UnityEngine;

public sealed class LootPickup : MonoBehaviour
{
    [SerializeField] private bool medkit;
    [SerializeField, Min(1f)] private float healAmount = 25f;
    [SerializeField, Min(1)] private int coinValue = 1;
    [SerializeField, Min(0.1f)] private float magnetRange = 3.5f;
    [SerializeField, Min(1f)] private float lifetime = 25f;
    [SerializeField, Min(0.01f)] private float pickupDistance = 0.55f;
    private Transform player;
    private float born;
    private Vector3 basePosition;
    private bool collected;
    public void InitializeCoin(int value) { coinValue = Mathf.Max(1, value); }

    private void Awake()
    {
        born = Time.time; basePosition = transform.position;
        ApplyColor(medkit ? new Color(0.9f, 0.95f, 1f) : new Color(1f, 0.72f, 0.08f));
    }
    private void Update()
    {
        if (collected) return;
        if (Time.time - born >= lifetime) { Destroy(gameObject); return; }
        if (player == null)
        {
            PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
            player = health != null ? health.transform : null;
        }
        transform.Rotate(0f, 90f * Time.deltaTime, 0f, Space.World);
        transform.position = basePosition + Vector3.up * (0.12f + Mathf.Sin((Time.time - born) * 3f) * 0.06f);
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= magnetRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position + Vector3.up * 0.6f, 8f * Time.deltaTime);
            if (distance <= pickupDistance) TryCollect();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerHealth>() != null) TryCollect();
    }
    private void TryCollect()
    {
        if (collected || player == null) return;
        if (GameSession.Instance != null && !GameSession.Instance.IsPlaying) return;
        CurrencyWallet wallet = FindFirstObjectByType<CurrencyWallet>();
        if (medkit)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health == null || health.IsDead || health.CurrentHealth >= health.MaxHealth) return;
            float before = health.CurrentHealth;
            health.Heal(healAmount);
            PickupFeedback.Show(transform.position, "+" + Mathf.CeilToInt(health.CurrentHealth - before) + " HP", new Color(0.42f, 1f, 0.55f));
        }
        else if (wallet != null)
        {
            wallet.AddCoins(coinValue);
            PickupFeedback.Show(transform.position, "+" + coinValue, new Color(1f, 0.78f, 0.16f));
        }
        else return;
        collected = true; PlayPickupSound(medkit); Destroy(gameObject);
    }
    private void ApplyColor(Color color)
    {
        var block = new MaterialPropertyBlock();
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.GetPropertyBlock(block); block.SetColor("_BaseColor", color); block.SetColor("_Color", color); renderer.SetPropertyBlock(block); }
    }
    private static void PlayPickupSound(bool heal)
    {
        int samples = 22050 / 12; AudioClip clip = AudioClip.Create(heal ? "MedkitPickup" : "CoinPickup", samples, 1, 22050, false); var data = new float[samples]; float f = heal ? 660f : 1050f; for (int i=0;i<samples;i++) data[i] = Mathf.Sin(2f*Mathf.PI*f*i/22050f) * (1f-i/(float)samples) * 0.16f; clip.SetData(data,0); AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero, 0.35f);
    }
}
