using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class EnemyViewAnimator : MonoBehaviour
{
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private Color criticalColor = new Color(1f, 0.18f, 0.16f, 1f);
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float deathDuration = 0.12f;

    private Enemy enemy;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Vector3 baseScale;
    private Vector3 previousPosition;
    private float hitTimer;
    private float deathTimer;
    private bool dying;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        baseScale = transform.localScale;
        previousPosition = transform.position;
    }

    private void OnEnable()
    {
        enemy.HealthChanged += HandleHealthChanged;
        enemy.Dying += HandleDying;
    }

    private void OnDisable()
    {
        enemy.HealthChanged -= HandleHealthChanged;
        enemy.Dying -= HandleDying;
    }

    private void Update()
    {
        if (dying)
        {
            deathTimer += Time.deltaTime;
            float t = Mathf.Clamp01(deathTimer / deathDuration);
            Color color = spriteRenderer.color;
            color.a = 1f - t;
            spriteRenderer.color = color;
            transform.localScale = Vector3.Lerp(baseScale, Vector3.zero, t);
            return;
        }

        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;

        if (delta.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * 12f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(baseScale.x * 1.12f, baseScale.y * 0.9f, baseScale.z), Time.deltaTime * 10f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.deltaTime * 10f);
        }

        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
            spriteRenderer.color = Color.Lerp(baseColor, hitFlashColor, Mathf.Clamp01(hitTimer / hitFlashDuration));
            return;
        }

        float healthRatio = enemy.MaxHealth > 0 ? enemy.CurrentHealth / (float)enemy.MaxHealth : 1f;
        spriteRenderer.color = healthRatio <= 0.3f ? Color.Lerp(baseColor, criticalColor, 0.45f) : baseColor;
    }

    private void HandleHealthChanged(Enemy changedEnemy, int current, int max)
    {
        if (current < max)
        {
            hitTimer = hitFlashDuration;
        }
    }

    private void HandleDying(Enemy dyingEnemy, bool grantReward)
    {
        dying = true;
        deathTimer = 0f;
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
    }
}
