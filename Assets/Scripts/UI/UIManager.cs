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

    public void Configure(Text coreText, Text goldValueText, Text waveValueText, GameObject gameOverRoot, CoreHealth core, WaveManager waves)
    {
        Unsubscribe();

        coreHealthText = coreText;
        goldText = goldValueText;
        waveText = waveValueText;
        gameOverPanel = gameOverRoot;
        coreHealth = core;
        waveManager = waves;

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
    }

    private void HandleGoldChanged(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold {gold}";
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(state == GameState.GameOver);
        }
    }
}
