using UnityEngine;

[RequireComponent(typeof(Enemy))]
public sealed class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.55f, 0f);
    [SerializeField] private Vector2 size = new Vector2(0.68f, 0.095f);
    [SerializeField] private Color backgroundColor = new Color(0.015f, 0.02f, 0.025f, 0.92f);
    [SerializeField] private Color healthyColor = new Color(0.2f, 1f, 0.38f, 1f);
    [SerializeField] private Color warningColor = new Color(1f, 0.78f, 0.12f, 1f);
    [SerializeField] private Color criticalColor = new Color(1f, 0.18f, 0.14f, 1f);

    private const float FillWidth = 0.84f;
    private const float FillHeight = 0.54f;

    private Enemy enemy;
    private Transform fill;
    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer fillRenderer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        CreateBar();
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

    private void LateUpdate()
    {
        if (backgroundRenderer != null)
        {
            Transform barTransform = backgroundRenderer.transform;
            barTransform.position = transform.position + localOffset;
            barTransform.rotation = Quaternion.identity;

            Vector3 parentScale = transform.lossyScale;
            barTransform.localScale = new Vector3(
                size.x / Mathf.Max(Mathf.Abs(parentScale.x), 0.001f),
                size.y / Mathf.Max(Mathf.Abs(parentScale.y), 0.001f),
                1f);
        }
    }

    private void CreateBar()
    {
        GameObject background = new GameObject("Enemy Health Bar");
        background.transform.SetParent(transform, false);
        background.transform.localPosition = localOffset;
        background.transform.localScale = new Vector3(size.x, size.y, 1f);
        backgroundRenderer = background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = CreateSprite();
        backgroundRenderer.color = backgroundColor;
        backgroundRenderer.sortingOrder = 10;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(background.transform, false);
        fillObject.transform.localPosition = Vector3.zero;
        fillObject.transform.localScale = new Vector3(FillWidth, FillHeight, 1f);
        fillRenderer = fillObject.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = backgroundRenderer.sprite;
        fillRenderer.color = healthyColor;
        fillRenderer.sortingOrder = 11;
        fill = fillObject.transform;
    }

    private void HandleHealthChanged(Enemy changedEnemy, int current, int max)
    {
        float ratio = max > 0 ? Mathf.Clamp01(current / (float)max) : 0f;
        fill.localScale = new Vector3(ratio * FillWidth, FillHeight, 1f);
        fill.localPosition = new Vector3((ratio - 1f) * FillWidth * 0.5f, 0f, 0f);
        fillRenderer.color = GetHealthColor(ratio);
        bool visible = ratio > 0f && ratio < 1f;
        backgroundRenderer.enabled = visible;
        fillRenderer.enabled = visible;
    }

    private void HandleDying(Enemy dyingEnemy, bool grantReward)
    {
        backgroundRenderer.enabled = false;
        fillRenderer.enabled = false;
    }

    private Color GetHealthColor(float ratio)
    {
        if (ratio > 0.5f)
        {
            return Color.Lerp(warningColor, healthyColor, (ratio - 0.5f) * 2f);
        }

        return Color.Lerp(criticalColor, warningColor, ratio * 2f);
    }

    private Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
