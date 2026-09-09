using UnityEngine;
using UnityEngine.UI;

public sealed class CurrencyHUD : MonoBehaviour
{
    [SerializeField] private Text currencyText;
    private CurrencyWallet wallet;
    public void Configure(Text text) => currencyText = text;
    private void Start()
    {
        if (currencyText == null)
        {
            GameObject go = new GameObject("CurrencyText", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(transform, false); currencyText = go.GetComponent<Text>();
            RectTransform rect = currencyText.rectTransform; rect.anchorMin = new Vector2(1f, 1f); rect.anchorMax = new Vector2(1f, 1f); rect.pivot = new Vector2(1f, 1f); rect.anchoredPosition = new Vector2(-28f, -28f); rect.sizeDelta = new Vector2(220f, 42f);
            currencyText.alignment = TextAnchor.MiddleRight; currencyText.fontSize = 24; currencyText.fontStyle = FontStyle.Bold; currencyText.color = new Color(1f, 0.82f, 0.2f); currencyText.raycastTarget = false;
        }
        wallet = FindFirstObjectByType<CurrencyWallet>(); if (wallet != null) { wallet.Changed += Refresh; Refresh(wallet.Coins); }
    }
    private void Refresh(int coins) { if (currencyText != null) currencyText.text = coins.ToString(); }
    private void OnDestroy() { if (wallet != null) wallet.Changed -= Refresh; }
}
