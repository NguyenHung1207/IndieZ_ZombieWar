using UnityEngine;

public sealed class WeaponOwnership : MonoBehaviour
{
    public const string Slot1Key = "ZombieWar.Loadout.Slot1";
    public const string Slot2Key = "ZombieWar.Loadout.Slot2";
    public static bool IsOwned(string id) => id == "assault_rifle" || id == "shotgun" || PlayerPrefs.GetInt("ZombieWar.WeaponOwned." + id, 0) == 1;
    public static void Unlock(string id) { if (!string.IsNullOrEmpty(id)) { PlayerPrefs.SetInt("ZombieWar.WeaponOwned." + id, 1); PlayerPrefs.Save(); } }
    public static string GetSlot(int slot) => PlayerPrefs.GetString(slot == 0 ? Slot1Key : Slot2Key, slot == 0 ? "assault_rifle" : "shotgun");
    public static void SetSlot(int slot, string id) { if (slot < 0 || slot > 1 || !IsOwned(id) || GetSlot(1 - slot) == id) return; PlayerPrefs.SetString(slot == 0 ? Slot1Key : Slot2Key, id); PlayerPrefs.Save(); }
    private void Awake()
    {
        string first = PlayerPrefs.GetString(Slot1Key, "assault_rifle");
        string second = PlayerPrefs.GetString(Slot2Key, "shotgun");
        if (!IsOwned(first)) first = "assault_rifle";
        if (!IsOwned(second) || second == first) second = first == "assault_rifle" ? "shotgun" : "assault_rifle";
        PlayerPrefs.SetString(Slot1Key, first); PlayerPrefs.SetString(Slot2Key, second); PlayerPrefs.Save();
    }
}
