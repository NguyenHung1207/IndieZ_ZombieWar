# Zombie War

## Overview

Zombie War is a focused top-down survival game created as a Unity recruitment-test project. It presents a complete Main Menu-to-result gameplay loop for desktop testing and landscape Android play.

## Unity Version

Unity 6000.3.15f1 LTS.

## Platform

Android, landscape orientation. The release APK targets ARM64 devices running Android 7.1 (API 25) or newer.

## Gameplay

Survive for 3 minutes while fighting progressively spawning Zombies. Enemy pressure increases through the session, up to a maximum of 24 active Zombies.

## Controls

Desktop:

- WASD — Move
- Mouse0 — Fire
- Q — Switch weapon
- G — Throw grenade

Mobile:

- Left joystick — Move
- FIRE — Fire
- SWITCH — Switch weapon
- GRENADE — Throw grenade

Firing uses forward-cone aim assist: the best visible living Zombie inside the cone is targeted, while blocked or rear targets are ignored. Movement and firing remain independent.

## Features

- Top-down Cinemachine follow camera with arena confinement
- CharacterController movement and layered upper/lower-body animation
- Automatic assault rifle and semi-automatic multi-pellet shotgun
- Weapon switching, recoil, muzzle flash, tracers, hit feedback, and combat audio
- Rigidbody grenade arc, collision, explosion damage, knockback, and cleanup
- AI Navigation/NavMesh Zombie chase and attack behavior
- Zombie hit/death animation, dissolve shader, and cleanup
- Progressive 180-second survival loop with HP, Victory, Game Over, Restart, and Main Menu return
- Responsive Safe Area-aware mobile HUD and touch controls

## How To Open

1. Install Unity Editor `6000.3.15f1` with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Open this repository folder as a Unity project.
3. Allow Unity to restore the locked packages without changing package versions.

## How To Play

Open `Assets/_Project/Scenes/MainMenu.unity`, enter Play Mode, and choose **PLAY**. The Main Menu is also scene 0 and is the release launch scene.

## Build

The deterministic release command is available at **Build > Zombie War > Build Android APK**. It validates the production scenes/assets and produces:

`Builds/Android/ZombieWar.apk`

The `Builds/` directory is intentionally ignored and the APK is a separate submission artifact. The equivalent command-line build is:

```powershell
& 'C:\C#\6000.3.15f1\Editor\Unity.exe' `
  -batchmode -quit `
  -projectPath 'F:\INDIEZ\IndieZ_ZombieWar' `
  -buildTarget Android `
  -executeMethod ReleaseBuild.BuildAndroid `
  -logFile 'F:\INDIEZ\IndieZ_ZombieWar\Logs\AndroidReleaseBuild.log'
```

Release configuration: package `com.indiez.zombiewar`, version `1.0.0` (version code `1`), IL2CPP, ARM64, minimum API 25, and target API Automatic Highest Installed. Development Build, Script Debugging, Autoconnect Profiler, and Deep Profiling are disabled by the build utility.

## Architecture

- `GameSession`, `ZombieSpawnDirector`, and `GameHUD` own the survival state, progressive spawn pressure, and presentation of session/player/weapon state.
- `PlayerMovement`, `PlayerAnimationController`, and `PlayerHealth` own player locomotion, layered animation parameters, and health.
- `PlayerWeaponController`, `PlayerAutoAim`, weapon definitions, recoil, and tracer components own the two-weapon firing pipeline.
- `PlayerGrenadeController` and `Grenade` own throwing, Rigidbody flight, explosion effects, damage, and knockback.
- `ZombieAI`, `ZombieHealth`, `ZombieAnimationController`, and `ZombieDissolve` own navigation, combat state, feedback, death, and cleanup.
- `VirtualJoystick`, mobile action buttons, `SafeAreaController`, and `MainMenuController` own touch/UI input and navigation.

## Third-Party Assets

The production build uses selected content from Survivalist Character, Zombie 1 (Low Poly), Guns Pack: Low Poly Guns Collection, and Toon Soldiers WW2 Demo. Source links and usage are documented in [THIRD_PARTY_ASSETS.md](THIRD_PARTY_ASSETS.md). These assets are not authored by this project and remain subject to their respective Unity Asset Store licenses.

## Known Limitations

- The release APK is ARM64-only and supports Android API 25 or newer.
- M13 automated Editor and APK validation completed without an attached Android device; physical touch, device Safe Area, and device performance checks remain part of the pre-submission manual checklist.

See [SUBMISSION_CHECKLIST.md](SUBMISSION_CHECKLIST.md) for final device checks and the gameplay-video capture plan.
