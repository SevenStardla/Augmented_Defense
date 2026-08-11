using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public sealed class AugmentManager : MonoBehaviour
{
    [SerializeField] private AugmentData[] availableAugments;
    [SerializeField] private int offerCount = 3;

    private readonly List<AugmentData> selectedAugments = new List<AugmentData>();
    private readonly Dictionary<string, int> stackCounts = new Dictionary<string, int>();

    public IReadOnlyList<AugmentData> SelectedAugments => selectedAugments;
    public event Action<AugmentData> AugmentSelected;

    public void Configure(AugmentData[] augments, int choices = 3)
    {
        availableAugments = augments;
        offerCount = Mathf.Max(1, choices);
    }

    public AugmentData[] RollOffers()
    {
        IEnumerable<AugmentData> pool = (availableAugments ?? Array.Empty<AugmentData>()).Where(CanSelect);
        return pool.OrderBy(_ => UnityEngine.Random.value).Take(offerCount).ToArray();
    }

    public void SelectAugment(AugmentData augment)
    {
        if (augment == null)
        {
            return;
        }

        if (!CanSelect(augment))
        {
            return;
        }

        selectedAugments.Add(augment);
        string key = augment.Key;
        stackCounts[key] = GetStackCount(augment) + 1;
        ApplyAugment(augment);
        AugmentSelected?.Invoke(augment);
        GameManager.Instance?.SetState(GameState.BuildPhase);
    }

    public int GetStackCount(AugmentData augment)
    {
        if (augment == null)
        {
            return 0;
        }

        return stackCounts.TryGetValue(augment.Key, out int count) ? count : 0;
    }

    private bool CanSelect(AugmentData augment)
    {
        if (augment == null)
        {
            return false;
        }

        if (augment.id == "core_repair")
        {
            CoreHealth core = FindFirstObjectByType<CoreHealth>();
            if (core == null || core.CurrentHealth >= core.MaxHealth)
            {
                return false;
            }
        }

        int currentStacks = GetStackCount(augment);
        int maxStacks = augment.canStack ? Mathf.Max(1, augment.maxStacks) : 1;
        return currentStacks < maxStacks;
    }

    private void ApplyAugment(AugmentData augment)
    {
        RuntimeAugmentStats stats = RuntimeAugmentStats.Instance;
        if (stats == null)
        {
            Debug.LogWarning("RuntimeAugmentStats is missing; augment could not be applied.");
            return;
        }

        switch (augment.id)
        {
            case "tower_damage":
                stats.AddTowerDamage(augment.value);
                break;
            case "tower_attack_speed":
                stats.AddTowerAttackSpeed(augment.value);
                break;
            case "tower_range":
                stats.AddTowerRange(augment.value);
                break;
            case "defender_damage":
                stats.AddDefenderDamage(augment.value);
                break;
            case "gold_reward":
                stats.AddGoldReward(augment.value);
                break;
            case "core_repair":
                FindFirstObjectByType<CoreHealth>()?.Heal(Mathf.RoundToInt(augment.value));
                break;
            default:
                Debug.LogWarning($"Unknown augment id: {augment.id}");
                break;
        }

        Debug.Log($"Selected augment: {augment.displayName} ({GetStackCount(augment)})");
    }
}
