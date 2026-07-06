using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Tower Data")]
public sealed class TowerData : ScriptableObject
{
    public TowerType type = TowerType.Basic;
    [Min(0)] public int cost = 50;
    [Min(0f)] public float damage = 10f;
    [Min(0.05f)] public float attackInterval = 1f;
    [Min(0.1f)] public float range = 4f;
}
