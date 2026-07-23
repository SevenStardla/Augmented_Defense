using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AugmentManager : MonoBehaviour
{
    [SerializeField] private AugmentData[] availableAugments;
    [SerializeField] private int offerCount = 3;

    private readonly List<AugmentData> selectedAugments = new List<AugmentData>();
    private readonly Dictionary<string, int> stackCounts = new Dictionary<string, int>();

    public IReadOnlyList<AugmentData> SelectedAugments => selectedAugments;

    public AugmentData[] RollOffers()
    {
        IEnumerable<AugmentData> pool = availableAugments.Where(CanSelect);
        return pool.OrderBy(_ => Random.value).Take(offerCount).ToArray();
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

        int currentStacks = GetStackCount(augment);
        int maxStacks = augment.canStack ? Mathf.Max(1, augment.maxStacks) : 1;
        return currentStacks < maxStacks;
    }

    private void ApplyAugment(AugmentData augment)
    {
        // MVP hook: concrete effects will be routed to tower/core/economy systems here.
        Debug.Log($"Selected augment: {augment.displayName}");
    }
}
