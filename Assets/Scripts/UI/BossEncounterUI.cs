using UnityEngine;
using UnityEngine.UI;

public sealed class BossEncounterUI : MonoBehaviour
{
    private EnemySpawner spawner;
    private Text announcementText;
    private float visibleTimer;
    private const float VisibleDuration = 3f;

    public void Configure(EnemySpawner enemySpawner, Text text)
    {
        spawner = enemySpawner;
        announcementText = text;

        if (spawner != null)
        {
            spawner.EnemySpawned += HandleEnemySpawned;
        }

        announcementText?.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemySpawned -= HandleEnemySpawned;
        }
    }

    private void Update()
    {
        if (visibleTimer <= 0f || announcementText == null)
        {
            return;
        }

        visibleTimer -= Time.unscaledDeltaTime;
        if (visibleTimer <= 0f)
        {
            announcementText.gameObject.SetActive(false);
        }
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        if (enemy == null || !enemy.IsBoss || announcementText == null)
        {
            return;
        }

        announcementText.text = $"WARNING\n{enemy.gameObject.name} INBOUND";
        announcementText.gameObject.SetActive(true);
        visibleTimer = VisibleDuration;
    }
}
