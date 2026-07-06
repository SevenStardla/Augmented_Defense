using System.Collections;
using UnityEngine;
using System;

public sealed class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float waveEndCheckInterval = 0.25f;

    public int CurrentWaveIndex { get; private set; }
    public int CurrentWaveNumber => CurrentWaveIndex + 1;

    private bool spawning;

    public event Action<int> WaveStarted;
    public event Action<int> WaveEnded;

    public void Configure(EnemySpawner enemySpawner, WaveData[] waveList)
    {
        spawner = enemySpawner;
        waves = waveList;
        CurrentWaveIndex = 0;
    }

    public void StartNextWave()
    {
        if (spawning || spawner == null || waves == null || CurrentWaveIndex >= waves.Length)
        {
            return;
        }

        StartCoroutine(RunWave(waves[CurrentWaveIndex]));
    }

    private IEnumerator RunWave(WaveData wave)
    {
        spawning = true;
        GameManager.Instance?.StartWave();
        WaveStarted?.Invoke(CurrentWaveNumber);

        yield return spawner.SpawnEnemies(wave.enemyCount, wave.enemyData);

        while (spawner.AliveCount > 0)
        {
            yield return new WaitForSeconds(waveEndCheckInterval);
        }

        spawning = false;
        WaveEnded?.Invoke(CurrentWaveNumber);
        CurrentWaveIndex++;

        if (CurrentWaveIndex >= waves.Length)
        {
            GameManager.Instance?.Clear();
        }
        else
        {
            GameManager.Instance?.EnterAugmentPhase();
        }
    }
}
