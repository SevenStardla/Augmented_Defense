using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Wave Data")]
public sealed class WaveData : ScriptableObject
{
    [Min(1)] public int enemyCount = 8;
    public EnemyData enemyData;
}
