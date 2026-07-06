using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private Text coreHealthText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text waveText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CoreHealth coreHealth;
    [SerializeField] private WaveManager waveManager;

    private bool subscribed;
    private UITextFeedback coreFeedback;
    private UITextFeedback goldFeedback;
    private UITextFeedback waveFeedback;
    private UIPanelFeedback gameOverFeedback;
    private int lastCoreHealth = -1;
    private int lastGold = -1;
    private GameState lastState;

    public void Configure(Text coreText, Text goldValueText, Text waveValueText, GameObject gameOverRoot, CoreHealth core, WaveManager waves)
    {
        Unsubscribe();

        coreHealthText = coreText;
        goldText = goldValueText;
        waveText = waveValueText;
        gameOverPanel = gameOverRoot;
        coreHealth = core;
        waveManager = waves;
        coreFeedback = coreHealthText != null ? coreHealthText.GetComponent<UITextFeedback>() : null;
        goldFeedback = goldText != null ? goldText.GetComponent<UITextFeedback>() : null;
        waveFeedback = waveText != null ? waveText.GetComponent<UITextFeedback>() : null;
        gameOverFeedback = gameOverPanel != null ? gameOverPanel.GetComponent<UIPanelFeedback>() : null;

        Subscribe();

        if (coreHealth != null)
        {
            HandleHealthChanged(coreHealth.CurrentHealth, coreHealth.MaxHealth);
        }

        if (EconomyManager.Instance != null)
        {
            HandleGoldChanged(EconomyManager.Instance.Gold);
        }

        HandleStateChanged(GameManager.Instance != null ? GameManager.Instance.State : GameState.BuildPhase);
    }

    public void NotifyPlacementFailed()
    {
        if (goldFeedback != null)
        {
            goldFeedback.PlayFlash(new Color(1f, 0.25f, 0.2f, 1f), 1.06f);
            goldFeedback.PlayShake();
        }
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (subscribed)
        {
            return;
        }

        if (coreHealth != null)
        {
            coreHealth.HealthChanged += HandleHealthChanged;
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.GoldChanged += HandleGoldChanged;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
        }

        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
        {
            return;
        }

        if (coreHealth != null)
        {
            coreHealth.HealthChanged -= HandleHealthChanged;
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.GoldChanged -= HandleGoldChanged;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }

        subscribed = false;
    }

    private void Update()
    {
        if (waveText != null && waveManager != null)
        {
            waveText.text = $"Wave {waveManager.CurrentWaveNumber}";
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (coreHealthText != null)
        {
            coreHealthText.text = $"Core {current}/{max}";
        }

        if (coreFeedback != null)
        {
            if (lastCoreHealth >= 0 && current < lastCoreHealth)
            {
                coreFeedback.PlayFlash(new Color(1f, 0.2f, 0.16f, 1f), 1.12f);
                coreFeedback.PlayShake();
            }

            coreFeedback.SetDanger(max > 0 && current > 0 && current / (float)max <= 0.3f);
            coreFeedback.SetDimmed(current <= 0);
        }

        lastCoreHealth = current;
    }

    private void HandleGoldChanged(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold {gold}";
        }

        if (goldFeedback != null)
        {
            if (lastGold >= 0 && gold > lastGold)
            {
                goldFeedback.PlayFlash(new Color(1f, 0.86f, 0.25f, 1f), 1.12f);
            }
            else if (lastGold >= 0 && gold < lastGold)
            {
                goldFeedback.PlayFlash(new Color(0.75f, 0.9f, 1f, 1f), 0.92f);
            }
        }

        lastGold = gold;
    }

    private void HandleStateChanged(GameState state)
    {
        if (gameOverPanel != null)
        {
            if (gameOverFeedback != null)
            {
                if (state == GameState.GameOver)
                {
                    gameOverFeedback.Show();
                }
                else
                {
                    gameOverFeedback.Hide();
                }
            }
            else
            {
                gameOverPanel.SetActive(state == GameState.GameOver);
            }
        }

        if (waveFeedback != null && state != lastState)
        {
            if (state == GameState.WavePhase)
            {
                waveFeedback.PlayFlash(new Color(1f, 0.86f, 0.3f, 1f), 1.2f);
            }
            else if (state == GameState.AugmentPhase || state == GameState.Clear)
            {
                waveFeedback.PlayFlash(Color.white, 1.1f);
            }
        }

        if (coreFeedback != null)
        {
            coreFeedback.SetDimmed(state == GameState.GameOver);
        }

        lastState = state;
    }
}
