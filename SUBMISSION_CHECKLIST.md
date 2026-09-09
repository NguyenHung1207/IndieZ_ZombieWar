# Zombie War Submission Checklist

## Release artifact

- [x] Unity: `6000.3.15f1`
- [x] Production scenes: MainMenu, then Gameplay_Level01 only
- [x] Android package: `com.indiez.zombiewar`
- [x] Version / code: `1.0.0` / `1`
- [x] IL2CPP, ARM64, landscape-only
- [x] Development Build, Script Debugging, and Autoconnect Profiler disabled
- [x] APK: `Builds/Android/ZombieWar.apk`
- [x] APK source commit: `19993a5` (M20 production source)
- [x] APK size: `123,814,408` bytes
- [x] APK modification time: `2026-09-09 08:22:17 +07:00`
- [x] APK SHA-256: `3BAD8F19E300D1A9837C33C772DB74756140788CD6CFBC61639A7D1DED26B062`
- [ ] GitHub sync: confirm local `main == origin/main`
- [ ] Gameplay video path or URL: `<ADD FINAL VIDEO PATH OR URL>`

## Release QA evidence

- [x] Main Menu, Shop, gameplay HUD, cooldown, pause, result flow, and responsive UI reviewed in Editor QA
- [x] Five-weapon Shop/loadout, ammo/reload, switching, grenade, loot, currency, pause, and 180-second session flows covered by project QA
- [x] Zombie AI, auto aim, combat grounding, NavMesh/world bounds, and runtime caps covered by project QA
- [x] Production asset/settings validation and Android release build completed: `BuildResult.Succeeded`, 0 errors, 1 warning
- [ ] Physical Android device test — **NOT PERFORMED** (no connected device)

## Device test, before submission

- [ ] APK installs and launches to Main Menu
- [ ] Both landscape rotations work; portrait is unavailable
- [ ] Joystick diagonal movement and simultaneous joystick + Fire/Grenade work
- [ ] Reload, Switch, grenade cooldown, Pause/Resume, Victory/Game Over, and audio work
- [ ] Physical Safe Area/notch layout is correct
- [ ] No severe FPS collapse or recurring logcat exception during a longer session

## Gameplay video

- [ ] Main Menu and Shop
- [ ] Movement, automatic and semi-automatic weapons, switching, reload, aim, and damage
- [ ] Grenade arc/explosion, Zombie chase/hit/death/dissolve, loot, and HUD
- [ ] Pause, Victory or Game Over, Restart, and Main Menu return

Do not describe physical-device QA as complete until it has actually been performed.
