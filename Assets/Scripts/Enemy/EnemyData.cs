using UnityEngine;

[CreateAssetMenu(menuName = "Augmented Defense/Enemy Data")]
public sealed class EnemyData : ScriptableObject
{
    public string displayName = "Enemy";
    public bool isBoss;
    public Sprite displaySprite;
    public Color displayColor = new Color(0.92f, 0.25f, 0.26f, 1f);
    [Min(0.25f)] public float sizeMultiplier = 1f;
    [Min(1)] public int maxHealth = 30;
    [Min(0.1f)] public float moveSpeed = 1f;
    [Min(0)] public int coreDamage = 10;
    [Min(0)] public int goldReward = 5;
}
