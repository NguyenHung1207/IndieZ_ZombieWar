# ZOMBIE WAR

Zombie War is a top-down zombie survival shooter for Android landscape and desktop testing. Survive two three-minute levels, collect loot and coins, and build a persistent two-slot weapon loadout.

## Features

- Level 1: flat infinite/recycled military battlefield
- Level 2: slope-based mutant stronghold
- Cinemachine top-down camera
- Five weapons, a persistent two-slot loadout, and weapon Shop
- Coins and loot; ammo, reload, recoil, tracers, VFX, and audio
- 360-degree automatic targeting and a physics grenade
- Normal, Male, Fat, and Giant Zombie variants with NavMesh chase, attack, death, and dissolve behavior
- Mobile joystick and action buttons
- Main Menu, Level Select, Shop, Settings, Pause, Victory, and Game Over

## Controls

### Desktop

- WASD — Move
- Mouse0 — Fire
- R — Reload
- Q — Switch
- G — Grenade
- Escape — Pause

### Mobile

- Joystick — Move
- Fire
- Reload
- Switch
- Grenade
- Pause

## Build

Unity `6000.3.15f1`

Android release settings:

- IL2CPP
- ARM64
- Landscape
- `com.indiez.zombiewar`
- Version `1.0.0` / code `1`

Build menu: `Build > Zombie War > Build Android APK`

Output: `Builds/Android/ZombieWar.apk`

## Production Scenes

1. `Assets/_Project/Scenes/MainMenu.unity`
2. `Assets/_Project/Scenes/Gameplay_Level01.unity`
3. `Assets/_Project/Scenes/Gameplay_Level02.unity`

## Third-Party Assets

See [THIRD_PARTY_ASSETS.md](THIRD_PARTY_ASSETS.md).
