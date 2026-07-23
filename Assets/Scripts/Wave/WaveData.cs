using UnityEngine;
using System;

[Serializable]
public struct WaveEntry
{
    public EnemyData enemyData;
    [Min(1)] public int count;
    [Min(0.01f)] public float spawnInterval;
}

[CreateAssetMenu(menuName = "Augmented Defense/Wave Data")]
public sealed class WaveData : ScriptableObject
{
    [Min(1)] public int enemyCount = 8;
    public EnemyData enemyData;
    public WaveEntry[] entries;

    public bool HasEntries => entries != null && entries.Length > 0;
}
