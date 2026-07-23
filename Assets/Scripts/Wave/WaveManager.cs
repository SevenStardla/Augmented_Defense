using System.Collections;
using UnityEngine;
using System;

public sealed class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float waveEndCheckInterval = 0.25f;

    public int CurrentWaveIndex { get; private set; }
    public int CurrentWaveNumber => waves != null && waves.Length > 0 ? Mathf.Min(CurrentWaveIndex + 1, waves.Length) : CurrentWaveIndex + 1;
    public int TotalWaves => waves != null ? waves.Length : 0;
    public bool IsWaveRunning => runningWave != null;

    private Coroutine runningWave;

    public event Action<int> WaveStarted;
    public event Action<int> WaveEnded;
    public event Action<int, int> WaveChanged;

    public void Configure(EnemySpawner enemySpawner, WaveData[] waveList)
    {
        spawner = enemySpawner;
        waves = waveList;
        CurrentWaveIndex = 0;
    }

    public void StartNextWave()
    {
        if (runningWave != null || spawner == null || waves == null || CurrentWaveIndex >= waves.Length)
        {
            return;
        }

        runningWave = StartCoroutine(RunWave(waves[CurrentWaveIndex]));
    }

    private IEnumerator RunWave(WaveData wave)
    {
        GameManager.Instance?.StartWave();
        WaveStarted?.Invoke(CurrentWaveNumber);
        WaveChanged?.Invoke(CurrentWaveNumber, TotalWaves);

        yield return spawner.SpawnWave(wave);

        while (spawner.IsSpawning || spawner.AliveCount > 0)
        {
            yield return new WaitForSeconds(waveEndCheckInterval);
        }

        WaveEnded?.Invoke(CurrentWaveNumber);
        CurrentWaveIndex++;
        WaveChanged?.Invoke(CurrentWaveNumber, TotalWaves);
        runningWave = null;

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
