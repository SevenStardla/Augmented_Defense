using UnityEngine;

[RequireComponent(typeof(CoreHealth))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class CoreViewAnimator : MonoBehaviour
{
    [SerializeField] private Color hitColor = new Color(1f, 0.25f, 0.2f, 1f);
    [SerializeField] private Color deadColor = new Color(0.12f, 0.12f, 0.14f, 1f);
    [SerializeField] private float breathAmount = 0.03f;
    [SerializeField] private float hitDuration = 0.2f;
    [SerializeField] private float shakeAmount = 0.08f;

    private CoreHealth coreHealth;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Vector3 baseScale;
    private Vector3 basePosition;
    private int lastHealth = -1;
    private float hitTimer;
    private bool dead;

    private void Awake()
    {
        coreHealth = GetComponent<CoreHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        baseScale = transform.localScale;
        basePosition = transform.position;
    }

    private void OnEnable()
    {
        coreHealth.HealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        coreHealth.HealthChanged -= HandleHealthChanged;
    }

    private void Update()
    {
        if (dead)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, deadColor, Time.unscaledDeltaTime * 8f);
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale * 0.85f, Time.unscaledDeltaTime * 8f);
            return;
        }

        float breath = 1f + Mathf.Sin(Time.time * 2.4f) * breathAmount;
        transform.localScale = baseScale * breath;

        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
            Vector2 random = Random.insideUnitCircle * shakeAmount * Mathf.Clamp01(hitTimer / hitDuration);
            transform.position = basePosition + new Vector3(random.x, random.y, 0f);
            spriteRenderer.color = hitColor;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, basePosition, Time.deltaTime * 14f);
        spriteRenderer.color = baseColor;
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (lastHealth >= 0 && current < lastHealth)
        {
            hitTimer = hitDuration;
        }

        dead = current <= 0;
        lastHealth = current;
    }
}
