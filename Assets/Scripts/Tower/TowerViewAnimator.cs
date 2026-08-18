using UnityEngine;

[RequireComponent(typeof(TowerAttack))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class TowerViewAnimator : MonoBehaviour
{
    [SerializeField] private Color fireFlashColor = new Color(1f, 0.32f, 0.06f, 1f);
    [SerializeField] private float fireFlashDuration = 0.16f;
    [SerializeField] private float fireScale = 1.25f;
    [SerializeField] private float rotateSpeed = 18f;

    private TowerAttack towerAttack;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Vector3 baseScale;
    private Transform currentTarget;
    private float fireTimer;

    private void Awake()
    {
        towerAttack = GetComponent<TowerAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        towerAttack.Fired += HandleFired;
    }

    private void OnDisable()
    {
        towerAttack.Fired -= HandleFired;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            }
        }

        if (fireTimer > 0f)
        {
            fireTimer -= Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fireTimer / fireFlashDuration);
            spriteRenderer.color = Color.Lerp(baseColor, fireFlashColor, t);
            transform.localScale = Vector3.Lerp(baseScale, baseScale * fireScale, t);
            return;
        }

        spriteRenderer.color = baseColor;
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.deltaTime * 18f);
    }

    private void HandleFired(Enemy target)
    {
        currentTarget = target != null ? target.transform : null;
        fireTimer = fireFlashDuration;
    }
}
