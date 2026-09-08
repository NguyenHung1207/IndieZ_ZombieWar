using UnityEditor;
using UnityEngine;

public static class M192QA
{
    const string Prefix="ZombieWar.WeaponOwned.";
    [MenuItem("Tools/Zombie War/QA/Grant 1000 Coins")]
    static void Grant1000(){PlayerPrefs.SetInt(CurrencyWallet.PlayerPrefsKey,1000);PlayerPrefs.Save();Debug.Log("QA: granted 1000 coins; reload scene to refresh scene-local wallet.");}
    [MenuItem("Tools/Zombie War/QA/Grant 10000 Coins")]
    static void Grant10000(){PlayerPrefs.SetInt(CurrencyWallet.PlayerPrefsKey,10000);PlayerPrefs.Save();Debug.Log("QA: granted 10000 coins; reload scene to refresh scene-local wallet.");}
    [MenuItem("Tools/Zombie War/QA/Reset Coins")]
    static void ResetCoins(){PlayerPrefs.SetInt(CurrencyWallet.PlayerPrefsKey,0);PlayerPrefs.Save();Debug.Log("QA: coins reset.");}
    [MenuItem("Tools/Zombie War/QA/Unlock All Weapons")]
    static void UnlockAll(){foreach(var id in new[]{"weapon_smg","weapon_pistol","weapon_sniper"})PlayerPrefs.SetInt(Prefix+id,1);PlayerPrefs.Save();Debug.Log("QA: unlocked all shop weapons.");}
    [MenuItem("Tools/Zombie War/QA/Reset Weapon Ownership")]
    static void ResetOwnership(){foreach(var id in new[]{"weapon_smg","weapon_pistol","weapon_sniper"})PlayerPrefs.DeleteKey(Prefix+id);PlayerPrefs.Save();Debug.Log("QA: ownership reset to default Rifle + Shotgun.");}
    [MenuItem("Tools/Zombie War/QA/Reset Loadout")]
    static void ResetLoadout(){PlayerPrefs.SetString(WeaponOwnership.Slot1Key,"assault_rifle");PlayerPrefs.SetString(WeaponOwnership.Slot2Key,"shotgun");PlayerPrefs.Save();Debug.Log("QA: loadout reset.");}
}
