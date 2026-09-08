# M15.5 Asset Library Intake

This is an intake/catalog milestone only. No imported asset is wired into gameplay by this milestone.

## Package archive inventory

| Package | Local Asset Store cache archive | Archive size | Archive file entries | Intake result |
|---|---|---:|---:|---|
| Zombie 1 Low Poly | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\ArtStore3D\3D ModelsCharactersHumanoidsHumans\Zombie 1 Low Poly.unitypackage` | 50.7 MiB | 72 | Existing production model; Pose model/prefab retained for future variants |
| Survivalist Character | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Slayver\3D ModelsCharacters\Survivalist character.unitypackage` | 204.4 MiB | 1,203 | Existing player art retained; StarterAssets/sample systems excluded |
| Guns Pack: Low Poly Guns Collection | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Fun Assets\3D ModelsPropsWeaponsGuns\Guns Pack Low Poly Guns Collection.unitypackage` | 93.4 MiB | 355 | All 12 model sets available; non-production variants imported |
| Military FREE Low Poly 3D Models | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\ithappy\3D ModelsEnvironments\Military FREE - Low Poly 3D Models Pack.unitypackage` | 34.8 MiB | 831 | Meshes, prefabs, materials, textures imported; demos/converters excluded |
| FREE Shirtless Zombie | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Studio New Punch\3D ModelsCharactersHumanoids\FREE Shirtless Zombie.unitypackage` | 90.0 MiB | 197 | URP model/material/prefab content imported; package has no animation clips |
| Toon Soldiers WW2 Demo | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Polygon Blacksmith\3D ModelsCharactersHumanoids\Toon Soldiers WW2 demo.unitypackage` | 0.75 MiB | 54 | Four animation FBXs retained/imported; no reload/grenade/hit/death clips present |
| Zombie Voice Audio Pack Free | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Tybug Studios\AudioSound FXCreatures\Zombie Voice Audio Pack - Free.unitypackage` | 5.5 MiB | 75 | Ten WAV clips imported |
| Post Apocalypse Guns Demo | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Sound Earth Game Audio\AudioSound FXWeapons\Post Apocalypse Guns Demo.unitypackage` | 5.8 MiB | 227 | 41 WAV clips imported; code remains unchanged |
| War FX | `C:\Users\NGUYEN HUNG\AppData\Roaming\Unity\Asset Store-5.x\Jean Moreno\Particle Systems\War FX.unitypackage` | 8.0 MiB | 2,188 | Mobile effects, dependencies, meshes, shaders, and runtime VFX helpers imported; demos excluded |

## Content and future use

### Zombie 1 Low Poly

Models: `Assets/ArtStore3D/Zombie/Model/Zombie.fbx` (current production) and `Zombie_Pose.fbx`; prefabs include `Zombie.prefab` and `Zombie_Pose.prefab`. One Zombie material and four texture maps are present. No animation clips are included. `Zombie_Pose` is available for future visual variety; SpawnDirector is unchanged.

### Survivalist Character

The package contains the base mesh, skin/gear material variants, body/equipment/FPS-arm textures, prefabs, and environment samples. The project retains the existing URP player subset and imported future material/gear variants. HDRP resources, StarterAssets, Input System assets, controller scripts, sample scenes, and package settings are intentionally excluded from the tracked intake.

### Guns Pack candidates

Every gun set contains an FBX, material, diffuse texture, and normal texture. Native orientation is the package's forward-facing low-poly presentation orientation; dimensions are compact prop scale and should be measured on wrapper prefabs before gameplay use.

| Candidate | Category | Package content | Future suitability |
|---|---|---|---|
| assault1 | assault rifle | FBX/material/diffuse/normal | Current production rifle |
| assault2, assault3, assault4 | assault rifle variants | FBX/material/diffuse/normal each | Shop candidates |
| shotgun1 | shotgun | FBX/material/diffuse/normal | Current production shotgun |
| shotgun2 | shotgun variant | FBX/material/diffuse/normal | Shop candidate |
| smg1, smg2 | SMG | FBX/material/diffuse/normal each | Shop candidates |
| pistol1, pistol2, pistol3, pistol4 | pistol | FBX/material/diffuse/normal each | Shop candidates |
| sniper1, sniper2 | sniper rifle | FBX/material/diffuse/normal each | Shop candidates |

The package's `Scenes/Main.unity` and `Scripts/GunsMenu.cs`/`GunView.cs` are excluded.

### Military FREE

Imported production-useful low-cost environment content under `Assets/ithappy/Military_Free`: buildings (radiostation, tents, tower), decorations (barrels, barriers, boxes/crates, cactus/grass, hedgehogs, mortar, tables, targets, tires), vehicle/installation parts (generator, helicopter, hummer, machine guns, protection, tank parts, doors/blades/wheels), frame, materials, and textures. These are reserved for M17 infinite-world dressing and M18 crate/loot dressing. Icons, demonstration scenes, skybox samples, and render-pipeline conversion packages are excluded.

### FREE Shirtless Zombie

The archive contains two model FBXs, body/clothes materials, URP/HDRP/Built-in material sets, six prefabs, textures, one optional eye-glow script, and three test scenes. It contains **no animation FBX/clip assets**, so there are no Idle/Walk/Run/Attack/Hit/Death/crawl clips to retarget. URP model content is imported for future zombie variety; test scenes and the optional script are excluded.

### Toon Soldiers WW2 Demo animations

| Clip source | Frames | Estimated duration at 30 fps | Loop |
|---|---:|---:|---|
| `animation/infantry_combat_idle.FBX` | 0–60 | 2.03 s | Yes |
| `animation/infantry_combat_run.FBX` | 0–25 | 0.87 s | Yes |
| `animation/infantry_combat_shoot.FBX` | 0–30 | 1.03 s | No |
| `animation/infantry_guard_idle.FBX` | 0–321 | 10.73 s | Yes |

No reload, grenade-throw, hit, or death clips are present. These FBXs remain animation-source content only; production Animator wiring is unchanged.

### Audio

Zombie Voice provides ten WAVs: aggressive (`zombie_agressive_039/044`), chase loop (`zombie_hyperchase_1_loop`), death (`zombie_death_004/010`), growl (`zombie_growl_010/023`), grunt, hiss, and moan. Durations are importer-readable WAV clips and are intentionally not rewritten into CombatAudio yet.

Post Apocalypse Guns provides assault-rifle `AutoGun_*`, shotgun `JackHammer_*`, pistol `Zapper_*`, sniper `AntiMaterialRifle_*`, far/third-person variants, and `Shotgun_Reload.wav`; no dedicated rifle reload/dry-fire/switch clip was found. `JackHammer_Reload.wav` is the best shotgun reload candidate; AutoGun 1p/3p clips are the best rifle candidates.

### War FX

The imported mobile subset includes bullet impacts (concrete/dirt/metal/sand/soft body/wood), explosions (simple/small/landmine/smoke), rifle muzzle flashes, fire/smoke, mobile materials/textures, meshes, shaders, and the small runtime fade/autodestruct helpers needed by prefabs. Recommended future comparisons: `WFXMR_MF 4P RIFLE1`, `WFXMR_MF Spr RIFLE1`, `WFXMR_BImpact SoftBody`, and `WFXMR_Explosion Small`. Desktop/demo scenes and demo/editor systems are excluded.

## Intake accounting

| Imported subset | Files (including metas) | Approx. disk size |
|---|---:|---:|
| Guns variants | 139 | 93.2 MiB |
| Military content | 216 | 8.5 MiB |
| Shirtless Zombie URP/model content | 40 | 91.5 MiB |
| Toon Soldiers animations | 25 | 1.9 MiB |
| Zombie Voice | 20 | 8.6 MiB |
| Post Apocalypse Guns audio | 82 | 6.7 MiB |
| War FX mobile/content subset | 372 | 13.8 MiB |
| **Total newly imported subset** | **894** | **224.2 MiB** |

Existing Zombie 1 and Survivalist production files were not duplicated. The full archives remain local cache inputs and are not vendored.

## Exclusions and compatibility

No Unity Input System package, StarterAssets controller, Cinemachine dependency, gameplay framework, demo scene, or package manifest was imported. Existing URP production scenes remain unchanged. Imported third-party materials are retained as authored; URP-compatible material variants were selected where available. No production Animator, weapon, audio, grenade, SpawnDirector, Player, or Zombie wiring changed in M15.5.

## Future mapping

- M16: compare War FX muzzle/impact/explosion effects and imported gun/reload audio.
- M17: use Military FREE environment props for infinite-world chunks.
- M18: use Military crates/boxes for loot dressing.
- M19: use unused Low Poly Guns variants as shop candidates.
- Zombie polish: review Shirtless Zombie model and existing Toon Soldiers animation sources; no new animation clips were available in Shirtless Zombie.
