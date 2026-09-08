using System.Collections.Generic;
using UnityEngine;

public enum CombatSound
{
    RifleShot,
    ShotgunShot,
    GrenadeExplosion,
    ZombieHit,
    ZombieDeath,
    ZombieVoice,
    ZombieAttack
}

[RequireComponent(typeof(AudioSource))]
public sealed class CombatAudio : MonoBehaviour
{
    private static readonly Dictionary<CombatSound, AudioClip> Clips = new Dictionary<CombatSound, AudioClip>();
    private static readonly Dictionary<CombatSound, string> ResourceNames = new Dictionary<CombatSound, string>
    {
        { CombatSound.RifleShot, "M16Audio/RifleShot" },
        { CombatSound.ShotgunShot, "M16Audio/ShotgunShot" },
        { CombatSound.ZombieVoice, "M16Audio/ZombieMoan" },
        { CombatSound.ZombieAttack, "M16Audio/ZombieAttack" },
        { CombatSound.ZombieHit, "M16Audio/ZombieHit" },
        { CombatSound.ZombieDeath, "M16Audio/ZombieDeath" }
    };
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0.75f;
        source.minDistance = 4f;
        source.maxDistance = 28f;
    }

    public void Play(CombatSound sound, float volume = 1f)
    {
        source.PlayOneShot(GetClip(sound), volume);
    }

    public void PlayReload(bool shotgun, float volume = 0.45f)
    {
        if (!shotgun)
            return;
        AudioClip clip = Resources.Load<AudioClip>("M16Audio/ShotgunReload");
        if (clip != null)
            source.PlayOneShot(clip, volume);
    }

    public void PlayWeapon(WeaponDefinition definition, float volume = 1f)
    {
        if (definition != null && definition.WeaponId == "weapon_sniper")
        {
            AudioClip sniper = Resources.Load<AudioClip>("M19Audio/SniperShot");
            if (sniper != null) { source.PlayOneShot(sniper, volume); return; }
        }
        Play(definition != null && definition.PelletCount > 1 ? CombatSound.ShotgunShot : CombatSound.RifleShot, volume);
    }

    public static void PlayAt(Vector3 position, CombatSound sound, float volume = 1f)
    {
        GameObject audioObject = new GameObject("CombatAudio_" + sound);
        audioObject.transform.position = position;
        CombatAudio audio = audioObject.AddComponent<CombatAudio>();
        AudioClip clip = GetClip(sound);
        audio.source.PlayOneShot(clip, volume);
        Destroy(audioObject, clip.length + 0.1f);
    }

    private static AudioClip GetClip(CombatSound sound)
    {
        if (Clips.TryGetValue(sound, out AudioClip clip))
            return clip;

        if (ResourceNames.TryGetValue(sound, out string resourceName))
        {
            clip = Resources.Load<AudioClip>(resourceName);
            if (clip != null)
            {
                Clips.Add(sound, clip);
                return clip;
            }
        }

        int sampleRate = 22050;
        float length = sound == CombatSound.GrenadeExplosion ? 0.65f : sound == CombatSound.ZombieVoice ? 0.42f : 0.16f;
        clip = AudioClip.Create("Generated_" + sound, Mathf.CeilToInt(sampleRate * length), 1, sampleRate, false);
        float[] samples = new float[clip.samples];
        float baseFrequency = sound == CombatSound.ShotgunShot ? 95f : sound == CombatSound.GrenadeExplosion ? 65f : sound == CombatSound.ZombieVoice || sound == CombatSound.ZombieAttack ? 150f : sound == CombatSound.ZombieDeath ? 110f : 240f;
        uint seed = (uint)(int)sound + 17u;
        for (int i = 0; i < samples.Length; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Pow(1f - i / (float)samples.Length, sound == CombatSound.ZombieVoice ? 1.4f : 3f);
            seed = seed * 1664525u + 1013904223u;
            float noise = ((seed >> 16) & 0xffff) / 32768f - 1f;
            float tone = Mathf.Sin(t * baseFrequency * Mathf.PI * 2f);
            samples[i] = (tone * 0.55f + noise * 0.45f) * envelope * (sound == CombatSound.GrenadeExplosion ? 0.8f : 0.45f);
        }
        clip.SetData(samples, 0);
        Clips.Add(sound, clip);
        return clip;
    }
}
