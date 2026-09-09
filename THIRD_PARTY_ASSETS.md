# Third-Party Assets

The production project uses selected files from the following Unity Asset Store packages. This project does not claim ownership of this art. Each asset remains subject to its respective Unity Asset Store license.

## Survivalist Character

- Source: [Survivalist Character](https://assetstore.unity.com/packages/3d/characters/survivalist-character-181470)
- Production use: player model, prefab, materials, and textures.

## Zombie 1 (Low Poly)

- Source: [Zombie 1](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/zombie-1-232270)
- Production use: Zombie model and textures. Project-authored material/controller behavior supplies the final combat presentation and dissolve integration.

## Guns Pack: Low Poly Guns Collection

- Source: [Guns Pack: Low Poly Guns Collection](https://assetstore.unity.com/packages/3d/props/guns/guns-pack-low-poly-guns-collection-192553)
- Production use: assault-rifle, shotgun, SMG, pistol, and sniper models/textures used by the five-weapon loadout and Shop previews.

## Toon Soldiers WW2 Demo

- Source: [Toon Soldiers WW2 Demo](https://assetstore.unity.com/packages/3d/characters/humanoids/toon-soldiers-ww2-demo-85702)
- Production use: Humanoid combat idle, run, and shoot animation sources plus their required model/material import dependencies.

## Military FREE Low Poly 3D Models

- Source: [Military FREE Low Poly 3D Models](https://assetstore.unity.com/packages/3d/environments/military-free-260358)
- Production use: military environment meshes, materials, and props used by the recycled battlefield. Demo scenes and conversion packages are not production scenes.

## FREE Shirtless Zombie

- Source: [FREE Shirtless Zombie](https://assetstore.unity.com/packages/3d/characters/humanoids/free-shirtless-zombie-276762)
- Intake use: URP model, materials, textures, and prefabs retained for future zombie visual variants. No package animation clips were present.

## Zombie Mutant Free

- Source: [Zombie Mutant Free](https://assetstore.unity.com/packages/3d/characters/humanoids/zombie-mutant-free-310842)
- Production use: selected mutant mesh, textures, materials, shader graph, and prefab feed the first-party Giant Zombie presentation wrapper. No package scene was imported.

## FREE Zombie Male AAB

- Source: [FREE Zombie Male AAB](https://assetstore.unity.com/packages/3d/characters/humanoids/free-zombie-male-aab-336744)
- Production use: selected URP prefab, mesh, URP materials, and textures feed the first-party male visual-variant wrapper. Demo scenes, HDRP content, and shaders were excluded.

## Fat Zombie (Low Poly)

- Source: [Fat Zombie (Low Poly)](https://assetstore.unity.com/packages/3d/characters/humanoids/fat-zombie-low-poly-296216)
- Production use: selected fat-zombie mesh, textures, material, and prefab feed the first-party heavy visual-variant wrapper. Demo, editor, animation, and documentation assets were excluded.

## 2D Gui Shooter + Icons

- Source: [2D Gui Shooter + Icons](https://assetstore.unity.com/packages/2d/gui/icons/2d-gui-shooter-icons-328881)
- Production use: `Icons/Icon_51.png` (Fire), `Icon_36.png` (Reload), `Icon_46.png` (Switch Weapon), `Icon_44.png` (Grenade), and `Icon_61.png` (Health). Demo scene and all unselected PNG assets were excluded.

## The Wasteland LITE

- Source: [The Wasteland LITE](https://assetstore.unity.com/packages/3d/environments/industrial/the-wasteland-lite-73054)
- Production use: `Fortified_Wall_1A`, `Wall_Broken_1A`, `Concrete_Block_1A`, and `Barrier_1A`, with their required FBX/material/texture dependencies. Package scenes, scripts, cameras, and legacy settings are excluded; props are decoration only and use no imported colliders.

## Zombie Voice Audio Pack Free

- Source: [Zombie Voice Audio Pack Free](https://assetstore.unity.com/packages/audio/sound-fx/creatures/zombie-voice-audio-pack-free-196645)
- Production use: selected moan, aggressive, grunt, and death WAV clips are loaded by the combat-audio presentation layer; other clips remain available for later tuning.

## Post Apocalypse Guns Demo

- Source: [Post Apocalypse Guns Demo](https://assetstore.unity.com/packages/audio/sound-fx/weapons/post-apocalypse-guns-demo-33515)
- Production use: AutoGun rifle, JackHammer shotgun, and JackHammer reload WAV clips are loaded by the combat-audio presentation layer; other variants remain available.

## War FX

- Source: [War FX](https://assetstore.unity.com/packages/vfx/particles/war-fx-5669)
- Production use: mobile bullet-impact, muzzle-flash, and small-explosion prefabs are referenced by first-party weapon/grenade adapters. Demo/editor systems are excluded.

Unity packages such as Universal Render Pipeline, Cinemachine, and AI Navigation are restored through `Packages/manifest.json` and `Packages/packages-lock.json`; they are not vendored Asset Store art.
