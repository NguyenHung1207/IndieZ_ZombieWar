# Zombie War Submission Checklist

## Release Artifact

- [x] GitHub repository: <https://github.com/NguyenHung1207/IndieZ_ZombieWar>
- [ ] Final release commit: record after the M13 commit
- [x] Unity version: `6000.3.15f1`
- [x] APK path: `Builds/Android/ZombieWar.apk`
- [x] APK size: `109,436,485` bytes
- [x] APK SHA-256: `A6A52E6343B5D0E34A2DDC38582E513DCC91291F607F209A2C1D84646AA39441`
- [x] Application identifier: `com.indiez.zombiewar`
- [x] Version / code: `1.0.0` / `1`
- [ ] Gameplay video path/link: `<ADD FINAL VIDEO PATH OR URL>`
- [x] README and third-party source documentation
- [ ] Confirm final `main == origin/main`

## Requirement Proof

- [x] Main Menu launch scene
- [x] 3-minute Level 1 survival loop
- [x] Virtual joystick and responsive Safe Area layout
- [x] Two guns and weapon switching
- [x] Recoil, muzzle flash, tracers, particles, and combat audio
- [x] Rigidbody grenade arc, damage, knockback, and cleanup
- [x] NavMesh Zombie AI, attacks, hit feedback, death, and dissolve
- [x] Upper/lower-body animation layers
- [x] Victory, Game Over, Restart, and Main Menu return
- [x] UI captures checked at 16:9, 18:9, 19.5:9, and 20:9

## Manual Android Device Checks

No Android device was connected during M13 automation. Complete these on the submission device:

- [ ] APK installs and launches directly to Main Menu
- [ ] Both landscape rotations work; portrait is unavailable
- [ ] Left joystick drag and diagonal movement
- [ ] Simultaneous joystick + FIRE
- [ ] Simultaneous joystick + GRENADE
- [ ] FIRE release stops automatic fire
- [ ] SWITCH while moving
- [ ] SWITCH while firing
- [ ] Shotgun remains semi-automatic
- [ ] Grenade arc, collision, fuse, explosion, damage, and cleanup
- [ ] Victory/Game Over RESTART and MAIN MENU buttons
- [ ] HP, timer, weapon, joystick, and action buttons respect the physical Safe Area
- [ ] No severe device FPS collapse, audio leaks, or repeated logcat exceptions during a longer session

## Gameplay Video Plan (2–4 minutes)

Keep menu footage brief and visibly prove the gameplay requirements in this order:

- [ ] A. Main Menu
- [ ] B. Choose PLAY
- [ ] C. Joystick movement, including diagonal movement
- [ ] D. Rifle automatic fire, forward-cone aim assist, muzzle flash, tracer, recoil, sound, and damage
- [ ] E. Weapon switching
- [ ] F. Shotgun semi-automatic fire and pellet spread
- [ ] G. Grenade throw arc, collision, and explosion
- [ ] H. Zombies spawning and chasing on the NavMesh
- [ ] I. Zombie hit feedback, death animation, and dissolve cleanup
- [ ] J. Player taking damage and HP HUD response
- [ ] K. Multiple Zombies and escalating pressure
- [ ] L. Victory or Game Over result panel
- [ ] M. Restart and return to Main Menu

Prefer natural production gameplay. If an Editor-only shortened timer is used only to demonstrate Victory, disclose that the production APK duration remains 180 seconds.
