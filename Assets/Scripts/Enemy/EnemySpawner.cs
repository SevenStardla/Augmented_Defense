using System.Collections;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EnemyData defaultEnemyData;
    [SerializeField] private Transform[] path;
    [SerializeField] private CoreHealth core;
    [SerializeField] private float spawnInterval = 0.75f;

    public int AliveCount { get; private set; }

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
        EnemyData data = enemyData != null ? enemyData : defaultEnemyData;

        for (int i = 0; i < count; i++)
        {
            Spawn(data);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn(EnemyData data)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner needs an enemy prefab.");
            return;
        }

        Enemy enemy = Instantiate(enemyPrefab, path != null && path.Length > 0 ? path[0].position : transform.position, Quaternion.identity);
        enemy.gameObject.SetActive(true);
        AliveCount++;
        enemy.Died += HandleEnemyRemoved;
        enemy.ReachedCore += HandleEnemyRemoved;
        enemy.Initialize(data, path, core);
    }

    private void HandleEnemyRemoved(Enemy enemy)
    {
        enemy.Died -= HandleEnemyRemoved;
        enemy.ReachedCore -= HandleEnemyRemoved;
        AliveCount = Mathf.Max(0, AliveCount - 1);
    }
}
