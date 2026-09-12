# ZOMBIE WAR

Zombie War is a top-down survival shooter for desktop and landscape Android. Survive three-minute runs across two distinct battlefields, defeat zombie hordes, collect loot and coins, and manage a persistent two-slot weapon loadout.

## Features

- Two 3-minute survival levels with Victory and Game Over
- Cinemachine top-down camera
- Level 1: flat, infinite recycled battlefield
- Level 2: slope-based stronghold battlefield with a Giant Zombie encounter
- Five weapons with persistent Shop and two-slot loadout
- Coins, loot drops, ammo, reload, recoil, muzzle VFX, tracers, and spatial audio
- 360-degree automatic target aiming
- Physics grenade with explosion damage, knockback, and cooldown
- Multiple zombie variants, including Male, Fat, and Giant Zombie enemies
- NavMesh Zombie chase/attack behavior with hit, death, dissolve, VFX, and audio
- Safe Area-aware mobile joystick plus Fire, Reload, Switch, Grenade, and Pause controls
- Main Menu, Level Select, Shop, Settings, Pause, Victory, and Game Over flows

## Controls

### Desktop

- WASD — Move
- Mouse0 — Fire
- R — Reload
- Q — Switch weapon
- G — Throw grenade
- Escape — Pause

### Mobile

- Joystick — Move
- Fire — Fire
- Reload — Reload
- Switch — Switch weapon
- Grenade — Throw grenade
- Pause — Pause

## Build

Use Unity `6000.3.15f1` with:

- Android Build Support
- Android SDK & NDK Tools
- OpenJDK

The release build command is:

`Build > Zombie War > Build Android APK`

Output:

`Builds/Android/ZombieWar.apk`

Release configuration:

- Application ID: `com.indiez.zombiewar`
- Version: `1.0.0`
- Version Code: `1`
- Scripting Backend: IL2CPP
- Architecture: ARM64
- Orientation: Landscape only
- Development Build: Disabled
- Script Debugging: Disabled
- Autoconnect Profiler: Disabled

## Production Scenes

1. `Assets/_Project/Scenes/MainMenu.unity`
2. `Assets/_Project/Scenes/Gameplay_Level01.unity`
3. `Assets/_Project/Scenes/Gameplay_Level02.unity`

## Third-Party Assets

Third-party production dependencies and their sources are documented in:

[THIRD_PARTY_ASSETS.md](THIRD_PARTY_ASSETS.md)
