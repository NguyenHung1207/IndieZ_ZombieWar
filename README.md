# IndieZ Zombie War

## Unity Version

6000.3.15f1

## Current Status

Work in progress recruitment test.

## Implemented

- CharacterController player locomotion
- top-down Cinemachine camera
- Survivalist visual
- Humanoid animation retargeting
- Idle/Run Blend Tree

## Controls

WASD in Editor.

## Project Architecture

The Player root owns gameplay movement and collision through PlayerMovement and CharacterController. The Visual child owns character rendering and the Animator. Cinemachine follows the Player root. Animation reads CharacterController velocity, and root motion is disabled.

## Third-Party Assets

See [THIRD_PARTY_ASSETS.md](THIRD_PARTY_ASSETS.md) for source references.

## Current Milestone

M3 complete.
"# IndieZ_ZombieWar" 
