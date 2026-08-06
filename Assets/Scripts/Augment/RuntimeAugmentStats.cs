using UnityEngine;

public sealed class RuntimeAugmentStats : MonoBehaviour
{
    public static RuntimeAugmentStats Instance { get; private set; }

    public float TowerDamageMultiplier { get; private set; } = 1f;
    public float TowerAttackSpeedMultiplier { get; private set; } = 1f;
    public float TowerRangeMultiplier { get; private set; } = 1f;
    public float DefenderDamageMultiplier { get; private set; } = 1f;
    public float GoldRewardMultiplier { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddTowerDamage(float amount) => TowerDamageMultiplier += Mathf.Max(0f, amount);
    public void AddTowerAttackSpeed(float amount) => TowerAttackSpeedMultiplier += Mathf.Max(0f, amount);
    public void AddTowerRange(float amount) => TowerRangeMultiplier += Mathf.Max(0f, amount);
    public void AddDefenderDamage(float amount) => DefenderDamageMultiplier += Mathf.Max(0f, amount);
    public void AddGoldReward(float amount) => GoldRewardMultiplier += Mathf.Max(0f, amount);
}
