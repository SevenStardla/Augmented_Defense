using UnityEngine;

[RequireComponent(typeof(DefenderController))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class DefenderViewAnimator : MonoBehaviour
{
    [SerializeField] private float tiltAngle = 6f;
    [SerializeField] private float attackScale = 1.08f;
    [SerializeField] private float attackPulseDuration = 0.08f;
    [SerializeField] private Color attackFlashColor = Color.white;

    private DefenderController controller;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Vector3 baseScale;
    private Vector2 moveInput;
    private float attackTimer;

    private void Awake()
    {
        controller = GetComponent<DefenderController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        controller.MoveInputChanged += HandleMoveInputChanged;
        controller.Fired += HandleFired;
    }

    private void OnDisable()
    {
        controller.MoveInputChanged -= HandleMoveInputChanged;
        controller.Fired -= HandleFired;
    }

    private void Update()
    {
        float targetAngle = -moveInput.x * tiltAngle;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, targetAngle), Time.deltaTime * 14f);

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(attackTimer / attackPulseDuration);
            transform.localScale = Vector3.Lerp(baseScale, baseScale * attackScale, t);
            spriteRenderer.color = Color.Lerp(baseColor, attackFlashColor, t);
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.deltaTime * 14f);
        spriteRenderer.color = baseColor;
    }

    private void HandleMoveInputChanged(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleFired(Enemy target)
    {
        attackTimer = attackPulseDuration;
    }
}
