using System.Collections;
using UnityEngine;
using System;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EnemyData defaultEnemyData;
    [SerializeField] private Transform[] path;
    [SerializeField] private CoreHealth core;
    [SerializeField] private float spawnInterval = 0.75f;

    public int AliveCount { get; private set; }
    public bool IsSpawning { get; private set; }
    public event Action<Vector3> SpawnWarning;
    public event Action<Enemy> EnemySpawned;
    public event Action<Enemy> EnemyRemoved;

    public void Configure(Enemy prefab, EnemyData enemyData, Transform[] waypointPath, CoreHealth targetCore, float interval)
    {
        enemyPrefab = prefab;
        defaultEnemyData = enemyData;
        path = waypointPath;
        core = targetCore;
        spawnInterval = interval;
    }

    public IEnumerator SpawnEnemies(int count, EnemyData enemyData = null)
    {
        IsSpawning = true;
        yield return SpawnBatch(count, enemyData, spawnInterval);
        IsSpawning = false;
    }

    public IEnumerator SpawnWave(WaveData wave)
    {
        IsSpawning = true;

        if (wave != null && wave.HasEntries)
        {
            foreach (WaveEntry entry in wave.entries)
            {
                yield return SpawnBatch(entry.count, entry.enemyData, entry.spawnInterval);
            }
        }
        else if (wave != null)
        {
            yield return SpawnBatch(wave.enemyCount, wave.enemyData, spawnInterval);
        }

        IsSpawning = false;
    }

    private IEnumerator SpawnBatch(int count, EnemyData enemyData, float interval)
    {
        EnemyData data = enemyData != null ? enemyData : defaultEnemyData;
        Vector3 spawnPosition = GetSpawnPosition();
        SpawnWarning?.Invoke(spawnPosition);

        for (int i = 0; i < count; i++)
        {
            Spawn(data);
            yield return new WaitForSeconds(Mathf.Max(0.01f, interval));
        }
    }

    private void Spawn(EnemyData data)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner needs an enemy prefab.");
            return;
        }

        Enemy enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
        enemy.gameObject.SetActive(true);
        AliveCount++;
        enemy.Died += HandleEnemyRemoved;
        enemy.Initialize(data, path, core);
        EnemySpawned?.Invoke(enemy);
    }

    private void HandleEnemyRemoved(Enemy enemy)
    {
        enemy.Died -= HandleEnemyRemoved;
        AliveCount = Mathf.Max(0, AliveCount - 1);
        EnemyRemoved?.Invoke(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        return path != null && path.Length > 0 ? path[0].position : transform.position;
    }
}
