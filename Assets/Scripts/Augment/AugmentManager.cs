using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AugmentManager : MonoBehaviour
{
    [SerializeField] private AugmentData[] availableAugments;
    [SerializeField] private int offerCount = 3;

    private readonly List<AugmentData> selectedAugments = new List<AugmentData>();

    public IReadOnlyList<AugmentData> SelectedAugments => selectedAugments;

    public AugmentData[] RollOffers()
    {
        IEnumerable<AugmentData> pool = availableAugments.Where(augment => augment != null && (augment.canStack || !selectedAugments.Contains(augment)));
        return pool.OrderBy(_ => Random.value).Take(offerCount).ToArray();
    }

    public void SelectAugment(AugmentData augment)
    {
        if (augment == null)
        {
            return;
        }

        if (!augment.canStack && selectedAugments.Contains(augment))
        {
            return;
        }

        selectedAugments.Add(augment);
        ApplyAugment(augment);
        GameManager.Instance?.SetState(GameState.BuildPhase);
    }

    private void ApplyAugment(AugmentData augment)
    {
        // MVP hook: concrete effects will be routed to tower/core/economy systems here.
        Debug.Log($"Selected augment: {augment.displayName}");
    }
}
