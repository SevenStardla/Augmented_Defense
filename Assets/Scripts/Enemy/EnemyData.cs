using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Enemy Data")]
public sealed class EnemyData : ScriptableObject
{
    [Min(1)] public int maxHealth = 30;
    [Min(0.1f)] public float moveSpeed = 1f;
    [Min(0)] public int coreDamage = 10;
    [Min(0)] public int goldReward = 5;
}
