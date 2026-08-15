using UnityEngine;
using UnityEngine.UI;

public sealed class BossHealthBarUI : MonoBehaviour
{
    private EnemySpawner spawner;
    private GameObject panel;
    private RectTransform fill;
    private Text label;
    private Enemy trackedBoss;

    public void Configure(EnemySpawner enemySpawner, GameObject panelRoot, RectTransform fillRect, Text valueLabel)
    {
        spawner = enemySpawner;
        panel = panelRoot;
        fill = fillRect;
        label = valueLabel;

        if (spawner != null)
        {
            spawner.EnemySpawned += HandleEnemySpawned;
        }

        panel?.SetActive(false);
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemySpawned -= HandleEnemySpawned;
        }

        StopTrackingBoss();
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        if (enemy == null || !enemy.IsBoss)
        {
            return;
        }

        StopTrackingBoss();
        trackedBoss = enemy;
        trackedBoss.HealthChanged += HandleHealthChanged;
        trackedBoss.Died += HandleBossRemoved;
        UpdateBar(trackedBoss.CurrentHealth, trackedBoss.MaxHealth);
        panel?.SetActive(true);
    }

    private void HandleHealthChanged(Enemy enemy, int current, int max)
    {
        if (enemy == trackedBoss)
        {
            UpdateBar(current, max);
        }
    }

    private void HandleBossRemoved(Enemy enemy)
    {
        if (enemy != trackedBoss)
        {
            return;
        }

        panel?.SetActive(false);
        StopTrackingBoss();
    }

    private void UpdateBar(int current, int max)
    {
        float ratio = max > 0 ? Mathf.Clamp01(current / (float)max) : 0f;

        if (fill != null)
        {
            fill.anchorMax = new Vector2(ratio, 1f);
        }

        if (label != null)
        {
            string bossName = trackedBoss != null ? trackedBoss.gameObject.name : "BOSS";
            label.text = $"{bossName}  {current}/{max}";
        }
    }

    private void StopTrackingBoss()
    {
        if (trackedBoss == null)
        {
            return;
        }

        trackedBoss.HealthChanged -= HandleHealthChanged;
        trackedBoss.Died -= HandleBossRemoved;
        trackedBoss = null;
    }
}
