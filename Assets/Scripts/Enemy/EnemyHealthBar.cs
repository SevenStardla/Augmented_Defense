using UnityEngine;

[RequireComponent(typeof(Enemy))]
public sealed class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.55f, 0f);
    [SerializeField] private Vector2 size = new Vector2(0.5f, 0.06f);
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Color fillColor = new Color(0.25f, 1f, 0.35f, 1f);

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
            backgroundRenderer.transform.rotation = Quaternion.identity;
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
        fillObject.transform.localScale = Vector3.one;
        fillRenderer = fillObject.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = backgroundRenderer.sprite;
        fillRenderer.color = fillColor;
        fillRenderer.sortingOrder = 11;
        fill = fillObject.transform;
    }

    private void HandleHealthChanged(Enemy changedEnemy, int current, int max)
    {
        float ratio = max > 0 ? Mathf.Clamp01(current / (float)max) : 0f;
        fill.localScale = new Vector3(ratio, 1f, 1f);
        fill.localPosition = new Vector3((ratio - 1f) * 0.5f, 0f, 0f);
        bool visible = ratio > 0f && ratio < 1f;
        backgroundRenderer.enabled = visible;
        fillRenderer.enabled = visible;
    }

    private void HandleDying(Enemy dyingEnemy, bool grantReward)
    {
        backgroundRenderer.enabled = false;
        fillRenderer.enabled = false;
    }

    private Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
