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
- Production use: the `assault1` and `shotgun1` models and textures. Other locally imported weapon variants are deferred and are not production dependencies.

## Toon Soldiers WW2 Demo

- Source: [Toon Soldiers WW2 Demo](https://assetstore.unity.com/packages/3d/characters/humanoids/toon-soldiers-ww2-demo-85702)
- Production use: Humanoid combat idle, run, and shoot animation sources plus their required model/material import dependencies.

## Military FREE Low Poly 3D Models

- Source: [Military FREE Low Poly 3D Models](https://assetstore.unity.com/packages/3d/environments/military-free-260358)
- Intake use: environment meshes, prefabs, materials, and textures retained for future world dressing; no demo scenes or conversion packages.

## FREE Shirtless Zombie

- Source: [FREE Shirtless Zombie](https://assetstore.unity.com/packages/3d/characters/humanoids/free-shirtless-zombie-276762)
- Intake use: URP model, materials, textures, and prefabs retained for future zombie visual variants. No package animation clips were present.

## Zombie Voice Audio Pack Free

- Source: [Zombie Voice Audio Pack Free](https://assetstore.unity.com/packages/audio/sound-fx/creatures/zombie-voice-audio-pack-free-196645)
- Intake use: selected zombie growl, chase, aggressive, hit-adjacent, and death WAV source clips; runtime audio wiring remains unchanged.

## Post Apocalypse Guns Demo

- Source: [Post Apocalypse Guns Demo](https://assetstore.unity.com/packages/audio/sound-fx/weapons/post-apocalypse-guns-demo-33515)
- Intake use: rifle, shotgun, pistol, sniper, far-field, and shotgun reload WAV source clips; runtime weapon audio remains unchanged.

## War FX

- Source: [War FX](https://assetstore.unity.com/packages/vfx/particles/war-fx-5669)
- Intake use: mobile bullet impacts, muzzle flashes, explosions, smoke/fire materials, textures, meshes, shaders, and required lightweight VFX helper scripts. Demo/editor systems are excluded.

Unity packages such as Universal Render Pipeline, Cinemachine, and AI Navigation are restored through `Packages/manifest.json` and `Packages/packages-lock.json`; they are not vendored Asset Store art.
