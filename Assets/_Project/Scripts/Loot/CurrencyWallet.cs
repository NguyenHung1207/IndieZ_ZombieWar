using System;
using UnityEngine;

public sealed class CurrencyWallet : MonoBehaviour
{
    public const string PlayerPrefsKey = "ZombieWar.Coins";
    public event Action<int> Changed;
    public int Coins { get; private set; }

    private void Awake() => Coins = Mathf.Max(0, PlayerPrefs.GetInt(PlayerPrefsKey, 0));
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount; Save(); Changed?.Invoke(Coins);
    }
    public bool CanAfford(int amount) => amount >= 0 && Coins >= amount;
    public bool TrySpend(int amount)
    {
        if (amount < 0 || Coins < amount) return false;
        Coins -= amount; Save(); Changed?.Invoke(Coins); return true;
    }
    private void Save() { PlayerPrefs.SetInt(PlayerPrefsKey, Coins); PlayerPrefs.Save(); }
}
