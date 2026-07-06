using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public sealed class SpawnPointWarningView : MonoBehaviour
{
    [SerializeField] private Color warningColor = new Color(1f, 0.25f, 0.2f, 0.75f);
    [SerializeField] private float visibleDuration = 0.45f;
    [SerializeField] private float radius = 0.55f;
    [SerializeField] private int segments = 48;

    private EnemySpawner spawner;
    private LineRenderer ring;
    private float timer;

    private void Awake()
    {
        spawner = GetComponent<EnemySpawner>();
        CreateRing();
    }

    private void OnEnable()
    {
        spawner.SpawnWarning += HandleSpawnWarning;
    }

    private void OnDisable()
    {
        spawner.SpawnWarning -= HandleSpawnWarning;
    }

    private void Update()
    {
        if (timer <= 0f)
        {
            ring.enabled = false;
            return;
        }

        timer -= Time.deltaTime;
        ring.enabled = true;
        float pulse = radius + Mathf.Sin(Time.time * 18f) * 0.08f;
        DrawRing(pulse);
    }

    private void CreateRing()
    {
        GameObject ringObject = new GameObject("Spawn Warning Ring");
        ringObject.transform.SetParent(transform, false);
        ring = ringObject.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = segments;
        ring.startWidth = 0.05f;
        ring.endWidth = 0.05f;
        ring.material = new Material(Shader.Find("Sprites/Default"));
        ring.startColor = warningColor;
        ring.endColor = warningColor;
        ring.enabled = false;
        DrawRing(radius);
    }

    private void DrawRing(float currentRadius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, 0f));
        }
    }

    private void HandleSpawnWarning(Vector3 position)
    {
        transform.position = position;
        timer = visibleDuration;
    }
}
