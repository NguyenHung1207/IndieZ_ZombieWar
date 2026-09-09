# ZOMBIE WAR

Zombie War is a top-down survival game for desktop testing and landscape Android play. Survive a three-minute run across an infinite recycled battlefield, defeat Zombies, collect loot, and manage a persistent two-slot weapon loadout.

## Features

- 3-minute top-down survival run with Victory and Game Over
- Cinemachine camera and an infinite recycled military battlefield
- Five weapons, persistent weapon Shop, coins, and loot drops
- Ammo, reload, recoil, muzzle VFX, spatial audio, and 360-degree auto aim
- Physics grenade with explosion damage, knockback, and an 8-second cooldown
- NavMesh Zombie chase/attack behavior with hit, death, dissolve, VFX, and audio
- Responsive Safe Area-aware mobile joystick and action controls

## Controls

Desktop:

- WASD — Move
- Mouse0 — Fire
- R — Reload
- Q — Switch weapon
- G — Throw grenade
- Escape — Pause

Mobile:

- Joystick — Move
- Fire — Fire
- Reload — Reload
- Switch — Switch weapon
- Grenade — Throw grenade
- Pause — Pause

## Build

Use Unity `6000.3.15f1` with Android Build Support, SDK/NDK Tools, and OpenJDK. The release build command is **Build > Zombie War > Build Android APK** and writes:

`Builds/Android/ZombieWar.apk`

Release settings: `com.indiez.zombiewar`, version `1.0.0` / code `1`, IL2CPP, ARM64, landscape-only, with Development Build, Script Debugging, and Autoconnect Profiler disabled.

Production scenes, in order:

1. `Assets/_Project/Scenes/MainMenu.unity`
2. `Assets/_Project/Scenes/Gameplay_Level01.unity`

Third-party production dependencies and sources are documented in [THIRD_PARTY_ASSETS.md](THIRD_PARTY_ASSETS.md). The release evidence and device/video checklist are in [SUBMISSION_CHECKLIST.md](SUBMISSION_CHECKLIST.md).
