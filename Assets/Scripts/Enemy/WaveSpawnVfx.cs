using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public sealed class WaveSpawnVfx : MonoBehaviour
{
    [SerializeField] private Color spawnColor = new Color(1f, 0.28f, 0.22f, 0.8f);
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private int segments = 40;

    private EnemySpawner spawner;

    private void Awake()
    {
        spawner = GetComponent<EnemySpawner>();
    }

    private void OnEnable()
    {
        spawner.EnemySpawned += HandleEnemySpawned;
    }

    private void OnDisable()
    {
        spawner.EnemySpawned -= HandleEnemySpawned;
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        GameObject ringObject = new GameObject("Enemy Spawn Ring");
        ringObject.transform.position = enemy.transform.position;
        SpawnRing ring = ringObject.AddComponent<SpawnRing>();
        ring.Configure(spawnColor, duration, segments);
    }

    private sealed class SpawnRing : MonoBehaviour
    {
        private LineRenderer line;
        private Color color;
        private float duration;
        private float elapsed;
        private int segments;

        public void Configure(Color ringColor, float ringDuration, int ringSegments)
        {
            color = ringColor;
            duration = ringDuration;
            segments = ringSegments;
            line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = segments;
            line.startWidth = 0.04f;
            line.endWidth = 0.04f;
            line.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float radius = Mathf.Lerp(0.1f, 0.65f, t);
            Color current = color;
            current.a *= 1f - t;
            line.startColor = current;
            line.endColor = current;

            for (int i = 0; i < segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            if (elapsed >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
