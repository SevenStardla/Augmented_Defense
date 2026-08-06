using UnityEngine;
using UnityEngine.UI;

public sealed class AugmentSelectionUI : MonoBehaviour
{
    private GameObject panel;
    private AugmentManager manager;
    private Button[] buttons;
    private Text[] labels;

    public void Configure(GameObject panelRoot, AugmentManager augmentManager, Button[] choiceButtons, Text[] choiceLabels)
    {
        panel = panelRoot;
        manager = augmentManager;
        buttons = choiceButtons;
        labels = choiceLabels;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
            HandleStateChanged(GameManager.Instance.State);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (state != GameState.AugmentPhase)
        {
            panel?.SetActive(false);
            return;
        }

        AugmentData[] offers = manager != null ? manager.RollOffers() : new AugmentData[0];
        panel?.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            button.onClick.RemoveAllListeners();
            bool hasOffer = i < offers.Length;
            button.gameObject.SetActive(hasOffer);

            if (!hasOffer)
            {
                continue;
            }

            AugmentData offer = offers[i];
            int nextStack = manager.GetStackCount(offer) + 1;
            labels[i].text = $"{offer.displayName}\n{offer.description}\nStack {nextStack}/{offer.maxStacks}";
            button.onClick.AddListener(() => manager.SelectAugment(offer));
        }
    }
}
