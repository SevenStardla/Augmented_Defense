using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class EnemyViewAnimator : MonoBehaviour
{
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private Color criticalColor = new Color(1f, 0.18f, 0.16f, 1f);
    [SerializeField] private float hitFlashDuration = 0.18f;
    [SerializeField] private float hitScaleMultiplier = 1.3f;
    [SerializeField] private float deathDuration = 0.28f;
    [SerializeField] private float deathSpinSpeed = 720f;

    private Enemy enemy;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Vector3 baseScale;
    private Vector3 initialScale;
    private Vector3 previousPosition;
    private float hitTimer;
    private float deathTimer;
    private bool dying;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        initialScale = transform.localScale;
        baseScale = initialScale;
        previousPosition = transform.position;
    }

    public void ApplyAppearance(Sprite sprite, Color color, float sizeMultiplier)
    {
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            baseColor = Color.white;
        }
        else
        {
            baseColor = color;
        }

        spriteRenderer.color = baseColor;
        baseScale = initialScale * Mathf.Max(0.25f, sizeMultiplier);
        transform.localScale = baseScale;
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
            deathTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(deathTimer / deathDuration);
            Color color = spriteRenderer.color;
            color.a = 1f - t;
            spriteRenderer.color = color;
            transform.localScale = Vector3.Lerp(baseScale, Vector3.zero, t);
            transform.Rotate(0f, 0f, deathSpinSpeed * Time.unscaledDeltaTime);
            return;
        }

        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;

        if (delta.sqrMagnitude > 0.0001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(baseScale.x * 1.12f, baseScale.y * 0.9f, baseScale.z), Time.deltaTime * 10f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.deltaTime * 10f);
        }

        if (hitTimer > 0f)
        {
            hitTimer -= Time.unscaledDeltaTime;
            float hitRatio = Mathf.Clamp01(hitTimer / hitFlashDuration);
            spriteRenderer.color = Color.Lerp(baseColor, hitFlashColor, hitRatio);
            transform.localScale = Vector3.Lerp(baseScale, baseScale * hitScaleMultiplier, hitRatio);
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
