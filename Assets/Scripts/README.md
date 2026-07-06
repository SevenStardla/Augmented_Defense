# Augmented Defense Scripts

This folder contains the MVP Unity script scaffold from `augment_defense_plan.md`.

Minimum scene setup:

1. Add `GameManager`, `EconomyManager`, `WaveManager`, `EnemySpawner`, `TowerPlacement`, and `UIManager` to scene objects.
2. Create `EnemyData`, `TowerData`, and `WaveData` assets from the `Augmented Defense` create menu.
3. Build an enemy prefab with `Enemy`, `EnemyMovement`, a `Collider2D`, and a visible sprite.
4. Build a tower prefab with `Tower`, `TowerAttack`, and a visible sprite.
5. Assign waypoint transforms to `EnemySpawner.path`; the last waypoint should be near the core.
6. Assign the core object with `CoreHealth`.

The first playable target is one placed tower attacking enemies moving along the waypoint path.

Quick play test:

1. Open `Assets/Scenes/Main.unity` and press Play.
2. `DemoBootstrap` creates a camera, player defender, enemy path, core, tower, wave manager, and UI automatically when no `GameManager` exists in the scene.
3. Controls: `WASD` moves the defender, `Space` shoots the nearest enemy in range, left click places an extra tower, and `Start Wave` begins spawning enemies.
